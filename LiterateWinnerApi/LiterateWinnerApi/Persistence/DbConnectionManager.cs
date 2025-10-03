using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Data.SqlClient;

namespace JobApplicationTrackerApi.Persistence;

/// <summary>
/// Enhanced connection manager for SQL Server with pooling optimizations
/// </summary>
public sealed class DbConnectionManager : IDisposable
{
    private readonly int _commandTimeout;
    private readonly SemaphoreSlim _connectionSemaphore;
    private readonly string _connectionString;
    private readonly Histogram<double>? _connectionTimeHistogram;
    private readonly ILogger<DbConnectionManager> _logger;
    private readonly int _maxConnections;
    private readonly ConcurrentDictionary<string, SqlConnection> _namedConnections = new();

    private bool _disposed;

    /// <summary>
    /// Constructor with configuration and logging
    /// </summary>
    /// <param name="configuration">Configuration containing connection strings and settings</param>
    /// <param name="logger">Logger for recording connection events and errors</param>
    /// <param name="meter">Optional meter for tracking connection performance metrics</param>
    /// <exception cref="InvalidOperationException">Thrown when DefaultConnection string is missing</exception>
    /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
    public DbConnectionManager(
        IConfiguration configuration,
        ILogger<DbConnectionManager> logger,
        Meter? meter = null)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new InvalidOperationException("DefaultConnection string is missing");

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure connection limits based on environment
        _maxConnections = Environment.ProcessorCount * 5; // Scale with available cores
        _connectionSemaphore = new SemaphoreSlim(_maxConnections, _maxConnections);
        _commandTimeout = 30; // Default timeout in seconds

        // Register metrics if meter is provided
        if (meter != null)
        {
            _connectionTimeHistogram = meter.CreateHistogram<double>("db_connection_time_ms");
        }

        // Log connection manager creation
        _logger.LogInformation("Database connection manager initialized with max connections: {MaxConnections}",
            _maxConnections);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Get a connection from the pool with advanced retry logic
    /// </summary>
    private async Task<SqlConnection> GetConnectionAsync(string? purpose = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // Implement connection limiting to prevent connection pool exhaustion
        if (!await _connectionSemaphore.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken))
        {
            _logger.LogWarning("Connection semaphore timeout reached. Connection pool may be exhausted.");
            throw new TimeoutException("Timeout waiting for database connection");
        }

        try
        {
            // For named connections (reuse for specific operations)
            if (!string.IsNullOrEmpty(purpose))
            {
                return await GetOrCreateNamedConnectionAsync(purpose, cancellationToken);
            }

            // For regular connections
            var connection = new SqlConnection(_connectionString);

            // Apply advanced connection settings
            connection.StatisticsEnabled = true;

            // Set command timeout using connection string parameter instead
            var builder = new SqlConnectionStringBuilder(connection.ConnectionString)
            {
                ConnectTimeout = _commandTimeout
            };
            connection.ConnectionString = builder.ConnectionString;

            try
            {
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Failed to open SQL connection: {Message}", ex.Message);
                connection.Dispose();
                throw;
            }
        }
        finally
        {
            stopwatch.Stop();
            _connectionTimeHistogram?.Record(stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Create or reuse a named connection for specific high-frequency operations
    /// </summary>
    private async Task<SqlConnection> GetOrCreateNamedConnectionAsync(string purpose,
        CancellationToken cancellationToken)
    {
        return await _namedConnections.GetOrAddAsync(purpose, async _ =>
        {
            var connection = new SqlConnection(_connectionString);

            // Apply optimized settings for named connections
            connection.StatisticsEnabled = true;

            // Set command timeout using connection string parameter instead
            var builder = new SqlConnectionStringBuilder(connection.ConnectionString)
            {
                ConnectTimeout = _commandTimeout
            };
            connection.ConnectionString = builder.ConnectionString;

            try
            {
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Failed to open named SQL connection: {Message}", ex.Message);
                connection.Dispose();
                throw;
            }
        });
    }

    /// <summary>
    /// Release a connection back to the pool
    /// </summary>
    private void ReleaseConnection(SqlConnection connection, bool keepAlive = false)
    {
        if (connection == null) return;

        // For named connections, keep them open but release semaphore
        if (keepAlive && _namedConnections.Values.Contains(connection))
        {
            _connectionSemaphore.Release();
            return;
        }

        // Otherwise close and dispose the connection
        try
        {
            connection.Close();
            connection.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing database connection: {Message}", ex.Message);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    /// <summary>
    /// Execute a database command with automatic connection management
    /// </summary>
    public async Task<T> ExecuteWithConnectionAsync<T>(
        Func<SqlConnection, Task<T>> action,
        string? purpose = null,
        CancellationToken cancellationToken = default)
    {
        SqlConnection? connection = null;

        try
        {
            connection = await GetConnectionAsync(purpose, cancellationToken);
            return await action(connection);
        }
        finally
        {
            if (connection != null)
            {
                ReleaseConnection(connection, keepAlive: !string.IsNullOrEmpty(purpose));
            }
        }
    }

    /// <summary>
    /// Get connection statistics to monitor performance
    /// </summary>
    public ConnectionStatistics GetStatistics()
    {
        return new ConnectionStatistics
        {
            ActiveConnections = _maxConnections - _connectionSemaphore.CurrentCount,
            MaxConnections = _maxConnections,
            NamedConnectionsCount = _namedConnections.Count
        };
    }

    /// <summary>
    /// Clear all connections from the pool
    /// </summary>
    public async Task ClearPoolAsync()
    {
        foreach (var connection in _namedConnections.Values)
        {
            try
            {
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing named connection during pool clear");
            }
        }

        _namedConnections.Clear();
        SqlConnection.ClearAllPools();

        await Task.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            foreach (var connection in _namedConnections.Values)
            {
                connection.Dispose();
            }

            _namedConnections.Clear();
            _connectionSemaphore.Dispose();
        }

        _disposed = true;
    }
}

/// <summary>
/// Connection statistics for monitoring
/// </summary>
public class ConnectionStatistics
{
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public int NamedConnectionsCount { get; set; }
}

/// <summary>
/// Extension method for ConcurrentDictionary to support async GetOrAdd
/// </summary>
public static class ConcurrentDictionaryExtensions
{
    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        var newValue = await valueFactory(key);
        return dictionary.GetOrAdd(key, _ => newValue);
    }
}