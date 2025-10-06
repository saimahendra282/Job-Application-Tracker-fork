namespace JobApplicationTrackerApi.Services.CacheService;

/// <summary>
/// Service for handling distributed and local caching
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get a cached item by key, or cache it if it doesn't exist
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null,
        bool useLocalCache = true);

    /// <summary>
    /// Get a cached item by key
    /// </summary>
    Task<T?> GetAsync<T>(string key, bool useLocalCache = true);

    /// <summary>
    /// Set a cached item by key
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Remove a cached item by key
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Remove all cached items by pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Acquire a distributed lock
    /// </summary>
    Task<IAsyncDisposable?> AcquireLockAsync(string key, TimeSpan? expiration = null);
}