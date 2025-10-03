using System.Diagnostics;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.IdentityContext;
using JobApplicationTrackerApi.Services.CacheService;
using Microsoft.EntityFrameworkCore;

// Add this NuGet package

namespace JobApplicationTrackerApi.Services.ConcurrencyService;

/// <summary>
/// Implementation of the concurrency service for handling database operations safely
/// </summary>
public class ConcurrencyService(
    DefaultContext dbContext,
    IdentityContext identityContext,
    ICacheService cacheService,
    ILogger<ConcurrencyService> logger
)
    : IConcurrencyService
{
    private readonly ICacheService
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

    private readonly DefaultContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly IdentityContext _identityContext =
        identityContext ?? throw new ArgumentNullException(nameof(identityContext));

    private readonly ILogger<ConcurrencyService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<TResult> ExecuteWithOptimisticConcurrencyAsync<TEntity, TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity, Task<TResult>> operation,
        int maxRetries = 3) where TEntity : class
    {
        int retryCount = 0;

        while (true)
        {
            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            try
            {
                return await executionStrategy.ExecuteAsync(async () =>
                {
                    // Start a new transaction
                    await using var transaction = await _dbContext.Database.BeginTransactionAsync();

                    // Find the entity with tracking
                    var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
                    if (entity == null)
                    {
                        throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} not found");
                    }

                    // Perform the operation
                    var result = await operation(entity);

                    // Save changes, which will check for concurrency conflicts
                    await _dbContext.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return result;
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;

                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex,
                        "Failed to execute operation after {RetryCount} retries due to concurrency conflict",
                        retryCount);
                    throw;
                }

                _logger.LogWarning(
                    "Concurrency conflict detected, retrying operation (attempt {RetryCount}/{MaxRetries})", retryCount,
                    maxRetries);

                // Clear the change tracker to prevent state conflicts on retry
                _dbContext.ChangeTracker.Clear();

                // Wait a bit before retrying to reduce contention
                await Task.Delay(50 * (int)Math.Pow(2, retryCount));
            }
        }
    }

    public async Task<TResult> ExecuteWithOptimisticConcurrencyIdentityAsync<TEntity, TResult>(
        Expression<Func<TEntity, bool>> predicate, Func<TEntity, Task<TResult>> operation,
        int maxRetries = 3) where TEntity : class
    {
        int retryCount = 0;

        while (true)
        {
            var executionStrategy = _identityContext.Database.CreateExecutionStrategy();
            try
            {
                return await executionStrategy.ExecuteAsync(async () =>
                {
                    // Start a new transaction
                    await using var transaction = await _identityContext.Database.BeginTransactionAsync();

                    // Find the entity with tracking
                    var entity = await _identityContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
                    if (entity == null)
                    {
                        throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} not found");
                    }

                    // Perform the operation
                    var result = await operation(entity);

                    // Save changes, which will check for concurrency conflicts
                    await _identityContext.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return result;
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;

                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex,
                        "Failed to execute operation after {RetryCount} retries due to concurrency conflict",
                        retryCount);
                    throw;
                }

                _logger.LogWarning(
                    "Concurrency conflict detected, retrying operation (attempt {RetryCount}/{MaxRetries})", retryCount,
                    maxRetries);

                // Clear the change tracker to prevent state conflicts on retry
                _identityContext.ChangeTracker.Clear();

                // Wait a bit before retrying to reduce contention
                await Task.Delay(50 * (int)Math.Pow(2, retryCount));
            }
        }
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteWithPessimisticConcurrencyAsync<TResult>(
        string lockKey,
        Func<Task<TResult>> operation,
        TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var lockAcquired = false;
        IAsyncDisposable? lockObj = null;

        try
        {
            // Try to acquire the distributed lock
            var startTime = DateTime.UtcNow;
            while (!lockAcquired && DateTime.UtcNow - startTime < timeout)
            {
                lockObj = await _cacheService.AcquireLockAsync(lockKey, timeout);
                if (lockObj != null)
                {
                    lockAcquired = true;
                    break;
                }

                // Wait a bit before retrying
                await Task.Delay(100);
            }

            if (!lockAcquired)
            {
                throw new TimeoutException($"Failed to acquire lock for key {lockKey} within timeout {timeout}");
            }

            // Perform the operation under the lock
            return await operation();
        }
        finally
        {
            // Release the lock if acquired
            if (lockAcquired && lockObj != null)
            {
                await lockObj.DisposeAsync();
            }
        }
    }

    public async Task<TResult> ExecuteWithPessimisticConcurrencyIdentityAsync<TResult>(string lockKey,
        Func<Task<TResult>> operation, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var lockAcquired = false;
        IAsyncDisposable? lockObj = null;

        try
        {
            // Try to acquire the distributed lock
            var startTime = DateTime.UtcNow;
            while (!lockAcquired && DateTime.UtcNow - startTime < timeout)
            {
                lockObj = await _cacheService.AcquireLockAsync(lockKey, timeout);
                if (lockObj != null)
                {
                    lockAcquired = true;
                    break;
                }

                // Wait a bit before retrying
                await Task.Delay(100);
            }

            if (!lockAcquired)
            {
                throw new TimeoutException($"Failed to acquire lock for key {lockKey} within timeout {timeout}");
            }

            // Perform the operation under the lock
            return await operation();
        }
        finally
        {
            // Release the lock if acquired
            if (lockAcquired && lockObj != null)
            {
                await lockObj.DisposeAsync();
            }
        }
    }

    /// <inheritdoc />
    public async Task ExecuteBatchAsync<TEntity, TItem>(
        IEnumerable<TItem> items,
        Func<IEnumerable<TItem>, Task> operation,
        int batchSize = 100) where TEntity : class
    {
        var itemsList = items.ToList();
        var totalItems = itemsList.Count;

        _logger.LogInformation("Processing {TotalItems} items in batches of {BatchSize}", totalItems, batchSize);

        for (int i = 0; i < totalItems; i += batchSize)
        {
            var batch = itemsList.Skip(i).Take(batchSize).ToList();

            try
            {
                await operation(batch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch {BatchNumber}/{TotalBatches}", i / batchSize + 1,
                    (totalItems + batchSize - 1) / batchSize);
                throw;
            }
        }
    }

    #region Bulk Operations - DefaultContext

    /// <summary>
    /// Executes bulk insert operation using DefaultContext with optimized performance
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkInsertAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation("Starting bulk insert of {TotalEntities} entities of type {EntityType}",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk insert");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            EnableShadowProperties = true,
            CalculateStats = true
        };

        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Disable change tracking for better performance
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _dbContext.BulkInsertAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk insert completed successfully in {ElapsedMilliseconds}ms for {Count} entities",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk insert operation");
                throw;
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    /// <summary>
    /// Executes bulk insert or update operation using DefaultContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert or update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkInsertOrUpdateAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation("Starting bulk insert or update of {TotalEntities} entities of type {EntityType}",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk insert or update");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            EnableShadowProperties = true,
            SetOutputIdentity = true,
            CalculateStats = true
        };

        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _dbContext.BulkInsertOrUpdateAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk insert or update completed successfully in {ElapsedMilliseconds}ms for {Count} entities",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk insert or update operation");
                throw;
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    /// <summary>
    /// Executes bulk update operation using DefaultContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkUpdateAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation("Starting bulk update of {TotalEntities} entities of type {EntityType}",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk update");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            CalculateStats = true
        };

        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _dbContext.BulkUpdateAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk update completed successfully in {ElapsedMilliseconds}ms for {Count} entities",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk update operation");
                throw;
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    #endregion

    #region Bulk Operations - IdentityContext

    /// <summary>
    /// Executes bulk insert operation using IdentityContext with optimized performance
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkInsertIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation(
            "Starting bulk insert of {TotalEntities} entities of type {EntityType} in Identity context",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk insert in Identity context");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            EnableShadowProperties = true,
            CalculateStats = true
        };

        var executionStrategy = _identityContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _identityContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                _identityContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _identityContext.BulkInsertAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk insert completed successfully in {ElapsedMilliseconds}ms for {Count} entities in Identity context",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk insert operation in Identity context");
                throw;
            }
            finally
            {
                _identityContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    /// <summary>
    /// Executes bulk insert or update operation using IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert or update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkInsertOrUpdateIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation(
            "Starting bulk insert or update of {TotalEntities} entities of type {EntityType} in Identity context",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk insert or update in Identity context");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            EnableShadowProperties = true,
            SetOutputIdentity = true,
            CalculateStats = true
        };

        var executionStrategy = _identityContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _identityContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                _identityContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _identityContext.BulkInsertOrUpdateAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk insert or update completed successfully in {ElapsedMilliseconds}ms for {Count} entities in Identity context",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk insert or update operation in Identity context");
                throw;
            }
            finally
            {
                _identityContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    /// <summary>
    /// Executes bulk update operation using IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    public async Task ExecuteBulkUpdateIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;

        _logger.LogInformation(
            "Starting bulk update of {TotalEntities} entities of type {EntityType} in Identity context",
            totalEntities, typeof(TEntity).Name);

        if (totalEntities == 0)
        {
            _logger.LogWarning("No entities provided for bulk update in Identity context");
            return;
        }

        bulkConfig ??= new BulkConfig
        {
            BatchSize = batchSize,
            BulkCopyTimeout = 300,
            CalculateStats = true
        };

        var executionStrategy = _identityContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _identityContext.Database.BeginTransactionAsync();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                _identityContext.ChangeTracker.AutoDetectChangesEnabled = false;

                await _identityContext.BulkUpdateAsync(entitiesList, bulkConfig);

                await transaction.CommitAsync();

                stopwatch.Stop();
                _logger.LogInformation(
                    "Bulk update completed successfully in {ElapsedMilliseconds}ms for {Count} entities in Identity context",
                    stopwatch.ElapsedMilliseconds, totalEntities);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during bulk update operation in Identity context");
                throw;
            }
            finally
            {
                _identityContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        });
    }

    #endregion

    #region Batch Processing with Bulk Operations

    /// <summary>
    /// Executes bulk operations in batches with retry logic for DefaultContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to process</param>
    /// <param name="bulkOperation">Bulk operation to execute</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <returns>Task</returns>
    public async Task ExecuteBatchBulkOperationAsync<TEntity>(
        IEnumerable<TEntity> entities,
        Func<IEnumerable<TEntity>, BulkConfig, Task> bulkOperation,
        int batchSize = 2000,
        int maxRetries = 3) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;
        var totalBatches = (int)Math.Ceiling((double)totalEntities / batchSize);

        _logger.LogInformation("Starting batch bulk operation for {TotalEntities} entities in {TotalBatches} batches",
            totalEntities, totalBatches);

        var bulkConfig = new BulkConfig
        {
            BatchSize = Math.Min(batchSize, 2000),
            BulkCopyTimeout = 300,
            EnableShadowProperties = true
        };

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = entitiesList.Skip(i * batchSize).Take(batchSize).ToList();
            var batchNumber = i + 1;

            _logger.LogInformation("Processing batch {BatchNumber}/{TotalBatches} with {BatchSize} entities",
                batchNumber, totalBatches, batch.Count);

            int retryCount = 0;
            while (retryCount <= maxRetries)
            {
                try
                {
                    await bulkOperation(batch, bulkConfig);
                    break; // Success, exit retry loop
                }
                catch (Exception ex) when (retryCount < maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning(ex,
                        "Batch {BatchNumber} failed, retrying... (attempt {RetryCount}/{MaxRetries})",
                        batchNumber, retryCount, maxRetries);

                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch {BatchNumber} failed after {MaxRetries} retries",
                        batchNumber, maxRetries);
                    throw;
                }
            }
        }

        _logger.LogInformation("Batch bulk operation completed successfully for all {TotalEntities} entities",
            totalEntities);
    }

    /// <summary>
    /// Executes bulk operations in batches with retry logic for IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to process</param>
    /// <param name="bulkOperation">Bulk operation to execute</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <returns>Task</returns>
    public async Task ExecuteBatchBulkOperationIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        Func<IEnumerable<TEntity>, BulkConfig, Task> bulkOperation,
        int batchSize = 2000,
        int maxRetries = 3) where TEntity : class
    {
        var entitiesList = entities.ToList();
        var totalEntities = entitiesList.Count;
        var totalBatches = (int)Math.Ceiling((double)totalEntities / batchSize);

        _logger.LogInformation(
            "Starting batch bulk operation for {TotalEntities} entities in {TotalBatches} batches in Identity context",
            totalEntities, totalBatches);

        var bulkConfig = new BulkConfig
        {
            BatchSize = Math.Min(batchSize, 2000),
            BulkCopyTimeout = 300,
            EnableShadowProperties = true
        };

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = entitiesList.Skip(i * batchSize).Take(batchSize).ToList();
            var batchNumber = i + 1;

            _logger.LogInformation(
                "Processing batch {BatchNumber}/{TotalBatches} with {BatchSize} entities in Identity context",
                batchNumber, totalBatches, batch.Count);

            int retryCount = 0;
            while (retryCount <= maxRetries)
            {
                try
                {
                    await bulkOperation(batch, bulkConfig);
                    break; // Success, exit retry loop
                }
                catch (Exception ex) when (retryCount < maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning(ex,
                        "Batch {BatchNumber} failed in Identity context, retrying... (attempt {RetryCount}/{MaxRetries})",
                        batchNumber, retryCount, maxRetries);

                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch {BatchNumber} failed in Identity context after {MaxRetries} retries",
                        batchNumber, maxRetries);
                    throw;
                }
            }
        }

        _logger.LogInformation(
            "Batch bulk operation completed successfully for all {TotalEntities} entities in Identity context",
            totalEntities);
    }

    #endregion
}