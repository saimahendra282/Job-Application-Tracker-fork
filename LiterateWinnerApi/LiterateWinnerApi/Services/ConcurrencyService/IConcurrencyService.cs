using System.Linq.Expressions;
using EFCore.BulkExtensions;

namespace JobApplicationTrackerApi.Services.ConcurrencyService;

/// <summary>
/// Service for handling concurrent database operations safely
/// </summary>
public interface IConcurrencyService
{
    /// <summary>
    /// Execute a database operation with optimistic concurrency control
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="predicate">The predicate to find the entity</param>
    /// <param name="operation">The operation to perform on the entity</param>
    /// <param name="maxRetries">Maximum number of retries in case of concurrency conflicts</param>
    /// <returns>The result of the operation</returns>
    Task<TResult> ExecuteWithOptimisticConcurrencyAsync<TEntity, TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity, Task<TResult>> operation,
        int maxRetries = 3) where TEntity : class;

    /// <summary>
    /// Execute a database operation with optimistic concurrency control for the Identity Context
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="predicate">The predicate to find the entity</param>
    /// <param name="operation">The operation to perform on the entity</param>
    /// <param name="maxRetries">Maximum number of retries in case of concurrency conflicts</param>
    /// <returns>The result of the operation</returns>
    /// <remarks>
    /// This method is specifically designed for operations on the Identity Context,
    /// which may have different concurrency requirements compared to other contexts.
    /// It allows for optimistic concurrency control while ensuring that the operation
    /// is performed safely within the Identity Context.
    /// </remarks>
    Task<TResult> ExecuteWithOptimisticConcurrencyIdentityAsync<TEntity, TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<TEntity, Task<TResult>> operation,
        int maxRetries = 3) where TEntity : class;

    /// <summary>
    /// Execute a database operation with pessimistic concurrency control (distributed lock)
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="lockKey">The key to use for locking</param>
    /// <param name="operation">The operation to perform under the lock</param>
    /// <param name="timeout">Maximum time to wait for the lock</param>
    /// <returns>The result of the operation</returns>
    Task<TResult> ExecuteWithPessimisticConcurrencyAsync<TResult>(
        string lockKey,
        Func<Task<TResult>> operation,
        TimeSpan? timeout = null);

    /// <summary>
    /// Execute a database operation with pessimistic concurrency control (distributed lock) for the Identity Context
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="lockKey">The key to use for locking</param>
    /// <param name="operation">The operation to perform under the lock</param>
    /// <param name="timeout">Maximum time to wait for the lock</param>
    /// <returns>The result of the operation</returns>
    /// <remarks>
    /// This method is specifically designed for operations on the Identity Context,
    /// which may have different concurrency requirements compared to other contexts.
    /// It allows for pessimistic concurrency control while ensuring that the operation
    /// is performed safely within the Identity Context.
    /// </remarks>
    Task<TResult> ExecuteWithPessimisticConcurrencyIdentityAsync<TResult>(
        string lockKey,
        Func<Task<TResult>> operation,
        TimeSpan? timeout = null);

    /// <summary>
    /// Execute a database operation in a batch for better performance
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <param name="items">The items to process</param>
    /// <param name="operation">The operation to perform on each batch</param>
    /// <param name="batchSize">The size of each batch</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ExecuteBatchAsync<TEntity, TItem>(
        IEnumerable<TItem> items,
        Func<IEnumerable<TItem>, Task> operation,
        int batchSize = 100) where TEntity : class;

    #region Bulk Operations - DefaultContext

    /// <summary>
    /// Executes bulk insert operation using DefaultContext with optimized performance
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    Task ExecuteBulkInsertAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

    /// <summary>
    /// Executes bulk insert or update operation using DefaultContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert or update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    Task ExecuteBulkInsertOrUpdateAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

    /// <summary>
    /// Executes bulk update operation using DefaultContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    Task ExecuteBulkUpdateAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

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
    Task ExecuteBulkInsertIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

    /// <summary>
    /// Executes bulk insert or update operation using IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to insert or update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    Task ExecuteBulkInsertOrUpdateIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

    /// <summary>
    /// Executes bulk update operation using IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to update</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="bulkConfig">Bulk configuration options</param>
    /// <returns>Task</returns>
    Task ExecuteBulkUpdateIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        int batchSize = 2000,
        BulkConfig? bulkConfig = null) where TEntity : class;

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
    Task ExecuteBatchBulkOperationAsync<TEntity>(
        IEnumerable<TEntity> entities,
        Func<IEnumerable<TEntity>, BulkConfig, Task> bulkOperation,
        int batchSize = 2000,
        int maxRetries = 3) where TEntity : class;

    /// <summary>
    /// Executes bulk operations in batches with retry logic for IdentityContext
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entities to process</param>
    /// <param name="bulkOperation">Bulk operation to execute</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <returns>Task</returns>
    Task ExecuteBatchBulkOperationIdentityAsync<TEntity>(
        IEnumerable<TEntity> entities,
        Func<IEnumerable<TEntity>, BulkConfig, Task> bulkOperation,
        int batchSize = 2000,
        int maxRetries = 3) where TEntity : class;

    #endregion
}