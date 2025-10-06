using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using JobApplicationTrackerApi.Persistence;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Services.CacheService;
using Microsoft.Extensions.Caching.Distributed;

namespace JobApplicationTrackerApi.Services.MonitoringService;

/// <summary>
/// Provides comprehensive monitoring metrics for the Social Casino API
/// </summary>
public class MonitoringService : IMonitoringService
{
    private readonly ObservableGauge<int> _activeDbConnections;
    private readonly Counter<long> _activeSessions;
    private readonly Counter<long> _apiRateLimitHits;
    private readonly Histogram<double> _apiResponseTimeHistogram;

    // Cache statistics
    private readonly ObservableGauge<double> _cacheHitRate;
    private readonly Histogram<double> _cacheHitTimeHistogram;
    private readonly Histogram<double> _cacheMissTimeHistogram;
    private readonly ICacheService _cacheService;
    private readonly ConcurrentDictionary<string, long> _cacheStats = new();
    private readonly DbConnectionManager _connectionManager;

    // Resource usage metrics
    private readonly ObservableGauge<double> _cpuUsage;
    private readonly Histogram<double> _dbQueryTimeHistogram;
    private readonly IDistributedCache _distributedCache;
    private readonly Counter<long> _errorCount;
    private readonly ILogger<MonitoringService> _logger;
    private readonly ObservableGauge<long> _memoryUsage;

    // Metrics
    private readonly Meter _meter;

    // Rate limiting for metrics collection
    private readonly TimeSpan _metricsInterval = TimeSpan.FromMinutes(5);
    private readonly Counter<long> _requestCount;
    private readonly IServiceProvider _serviceProvider;
    private readonly ObservableGauge<long> _threadCount;

    // Performance tracker
    private readonly Stopwatch _uptime = Stopwatch.StartNew();
    private DateTime _lastMetricsUpdate = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringService"/> class.
    /// Sets up all monitoring metrics, counters, and observable gauges for the application.
    /// </summary>
    /// <param name="logger">The logger for recording service events and errors</param>
    /// <param name="serviceProvider">Service provider for obtaining scoped services</param>
    /// <param name="cacheService">Service for cache operations and monitoring</param>
    /// <param name="distributedCache">Distributed cache implementation</param>
    /// <param name="connectionManager">Database connection manager for tracking DB stats</param>
    /// <exception cref="ArgumentNullException">Thrown when any required dependency is null</exception>
    public MonitoringService(
        ILogger<MonitoringService> logger,
        IServiceProvider serviceProvider,
        ICacheService cacheService,
        IDistributedCache distributedCache,
        DbConnectionManager connectionManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));

        // Create meter for all metrics - using less frequent collection
        _meter = new Meter("ChatBotApi.Monitoring", "1.0.0");

        // Register performance metrics
        _apiResponseTimeHistogram = _meter.CreateHistogram<double>("api_response_time_ms");
        _dbQueryTimeHistogram = _meter.CreateHistogram<double>("db_query_time_ms");
        _cacheHitTimeHistogram = _meter.CreateHistogram<double>("cache_hit_time_ms");
        _cacheMissTimeHistogram = _meter.CreateHistogram<double>("cache_miss_time_ms");

        // Register counters
        _activeSessions = _meter.CreateCounter<long>("active_sessions");
        _requestCount = _meter.CreateCounter<long>("request_count");
        _errorCount = _meter.CreateCounter<long>("error_count");
        _apiRateLimitHits = _meter.CreateCounter<long>("api_rate_limit_hits");

        // Register resource usage metrics with reduced frequency using observable gauges
        _cpuUsage = _meter.CreateObservableGauge("cpu_usage_percent", () =>
            ShouldCollectMetrics() ? GetCpuUsage() : 0);

        _memoryUsage = _meter.CreateObservableGauge("memory_usage_bytes", () =>
            ShouldCollectMetrics() ? GetMemoryUsage() : 0);

        _activeDbConnections = _meter.CreateObservableGauge("active_db_connections", () =>
            ShouldCollectMetrics() ? GetActiveDbConnections() : 0);

        _threadCount = _meter.CreateObservableGauge("thread_count", () =>
            ShouldCollectMetrics() ? GetThreadCount() : 0);

        // Register cache metrics
        _cacheHitRate = _meter.CreateObservableGauge("cache_hit_rate", () =>
            ShouldCollectMetrics() ? GetCacheHitRate() : 0);

        _logger.LogInformation("Monitoring service initialized with reduced metrics collection frequency");
    }

    /// <summary>
    /// Record API request start
    /// </summary>
    public void RecordRequestStart(string endpoint, string method)
    {
        _requestCount.Add(1);
        _cacheStats.AddOrUpdate($"request:{endpoint}:{method}", 1, (_, count) => count + 1);
    }

    /// <summary>
    /// Record API request completion with timing
    /// </summary>
    public void RecordRequestEnd(string endpoint, string method, double elapsedMs, bool isSuccess)
    {
        _apiResponseTimeHistogram.Record(elapsedMs);

        if (!isSuccess)
        {
            _errorCount.Add(1);
            _cacheStats.AddOrUpdate("errors", 1, (_, count) => count + 1);
        }

        _cacheStats.AddOrUpdate($"response_time:{endpoint}:{method}",
            (long)elapsedMs,
            (_, current) => (current + (long)elapsedMs) / 2); // Rolling average
    }

    /// <summary>
    /// Record database query execution time
    /// </summary>
    public void RecordDbQueryTime(string queryName, double elapsedMs)
    {
        _dbQueryTimeHistogram.Record(elapsedMs);
        _cacheStats.AddOrUpdate($"db_query:{queryName}",
            (long)elapsedMs,
            (_, current) => (current + (long)elapsedMs) / 2); // Rolling average
    }

    /// <summary>
    /// Record cache hit
    /// </summary>
    public void RecordCacheHit(string cacheKey, double elapsedMs)
    {
        _cacheHitTimeHistogram.Record(elapsedMs);
        _cacheStats.AddOrUpdate("cache_hits", 1, (_, count) => count + 1);
        _cacheStats.AddOrUpdate($"cache_hit:{cacheKey}", 1, (_, count) => count + 1);
    }

    /// <summary>
    /// Record cache miss
    /// </summary>
    public void RecordCacheMiss(string cacheKey, double elapsedMs)
    {
        _cacheMissTimeHistogram.Record(elapsedMs);
        _cacheStats.AddOrUpdate("cache_misses", 1, (_, count) => count + 1);
        _cacheStats.AddOrUpdate($"cache_miss:{cacheKey}", 1, (_, count) => count + 1);
    }

    /// <summary>
    /// Record session start
    /// </summary>
    public void RecordSessionStart(string userId)
    {
        _activeSessions.Add(1);
        _cacheStats.AddOrUpdate($"session:{userId}", 1, (_, count) => count);
        _cacheStats.AddOrUpdate("active_sessions", 1, (_, count) => count + 1);
    }

    /// <summary>
    /// Record session end
    /// </summary>
    public void RecordSessionEnd(string userId)
    {
        _activeSessions.Add(-1);
        _cacheStats.TryRemove($"session:{userId}", out _);
        _cacheStats.AddOrUpdate("active_sessions", 0, (_, count) => Math.Max(0, count - 1));
    }

    /// <summary>
    /// Record API rate limit hit
    /// </summary>
    public void RecordRateLimitHit(string endpoint, string clientIp)
    {
        _apiRateLimitHits.Add(1);
        _cacheStats.AddOrUpdate($"rate_limit:{endpoint}", 1, (_, count) => count + 1);
        _cacheStats.AddOrUpdate($"rate_limit_ip:{clientIp}", 1, (_, count) => count + 1);
    }

    /// <summary>
    /// Get monitoring statistics for dashboard
    /// </summary>
    public async Task<MonitoringStatistics> GetStatisticsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

            // Calculate cache hit rate
            var cacheHitRate = GetCacheHitRate();

            // Get DB connection stats
            var connectionStats = _connectionManager.GetStatistics();

            // Calculate response time percentiles
            var avgResponseTime = _cacheStats
                .Where(kv => kv.Key.StartsWith("response_time:"))
                .Select(kv => kv.Value)
                .DefaultIfEmpty(0)
                .Average();

            return new MonitoringStatistics
            {
                ActiveSessions = _cacheStats.GetValueOrDefault("active_sessions"),
                TotalRequests = GetCounterValue(_requestCount),
                ErrorRate = GetCounterValue(_requestCount) > 0
                    ? (double)GetCounterValue(_errorCount) / GetCounterValue(_requestCount)
                    : 0,
                AverageResponseTimeMs = avgResponseTime,
                CacheHitRate = cacheHitRate,
                ActiveDbConnections = connectionStats.ActiveConnections,
                MaxDbConnections = connectionStats.MaxConnections,
                CpuUsagePercent = GetCpuUsage(),
                MemoryUsageMb = GetMemoryUsage() / (1024 * 1024),
                ThreadCount = GetThreadCount(),
                UptimeSeconds = _uptime.Elapsed.TotalSeconds,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monitoring statistics");
            return new MonitoringStatistics();
        }
    }

    /// <summary>
    /// Reset counters for new monitoring period
    /// </summary>
    public void ResetCounters()
    {
        // Only reset certain statistics, keep cumulative ones
        var keysToRemove = _cacheStats
            .Where(kv => kv.Key.StartsWith("rate_limit:") || kv.Key.StartsWith("response_time:"))
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cacheStats.TryRemove(key, out _);
        }

        _logger.LogInformation("Reset monitoring counters");
    }

    /// <summary>
    /// Determines if metrics should be collected based on interval
    /// </summary>
    private bool ShouldCollectMetrics()
    {
        var now = DateTime.UtcNow;
        if (now - _lastMetricsUpdate < _metricsInterval)
        {
            return false;
        }

        _lastMetricsUpdate = now;
        return true;
    }

    /// <summary>
    /// Helper method to get counter value since Counter<T> doesn't expose GetCurrentValue()
    /// </summary>
    private long GetCounterValue(Counter<long> counter)
    {
        // Workaround for Counter<T> not exposing current value
        // In a real implementation, use a separate variable to track values
        // or use OpenTelemetry API to retrieve the actual value
        string name = counter.ToString() ?? "";
        if (name.Contains("error"))
            return _cacheStats.GetValueOrDefault("errors");
        if (name.Contains("request"))
            return _cacheStats.Values.Where((_, i) => i % 2 == 0).Sum(); // Approximate

        return 0; // Fallback
    }

    /// <summary>
    /// Get estimated CPU usage
    /// </summary>
    private double GetCpuUsage()
    {
        try
        {
            // Simple approximation based on process CPU time
            var process = Process.GetCurrentProcess();

            // Use processor time as percentage of elapsed time
            var cpuUsage = process.TotalProcessorTime.TotalMilliseconds /
                (Environment.ProcessorCount * _uptime.ElapsedMilliseconds) * 100;

            return Math.Min(100, cpuUsage); // Cap at 100%
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get current memory usage
    /// </summary>
    private long GetMemoryUsage()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            return process.WorkingSet64;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get active DB connections
    /// </summary>
    private int GetActiveDbConnections()
    {
        try
        {
            var stats = _connectionManager.GetStatistics();
            return stats.ActiveConnections;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get current thread count
    /// </summary>
    private long GetThreadCount()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            return process.Threads.Count;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Calculate cache hit rate
    /// </summary>
    private double GetCacheHitRate()
    {
        long hits = _cacheStats.GetValueOrDefault("cache_hits");
        long misses = _cacheStats.GetValueOrDefault("cache_misses");

        if (hits + misses == 0)
            return 0;

        return (double)hits / (hits + misses);
    }
}