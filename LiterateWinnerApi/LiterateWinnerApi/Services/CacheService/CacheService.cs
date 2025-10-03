using System.Text;
using System.Text.Json;
using JobApplicationTrackerApi.Services.CacheService;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace LiterateWinnerApi.Services.CacheService;

/// <summary>
/// Implementation of the cache service using Redis for distributed caching and memory cache for local caching
/// </summary>
public class CacheService(
    IDistributedCache distributedCache,
    IMemoryCache memoryCache,
    IServiceProvider serviceProvider,
    ILogger<CacheService> logger
)
    : ICacheService
{
    // Use shorter cache times for frequently changing data like comments
    private readonly DistributedCacheEntryOptions _commentOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    private readonly DistributedCacheEntryOptions _defaultOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    private readonly IDistributedCache _distributedCache =
        distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<CacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    private readonly IConnectionMultiplexer? _redis = serviceProvider.GetService<IConnectionMultiplexer>();

    private readonly IDatabase? _redisDb = serviceProvider.GetService<IConnectionMultiplexer>()?.GetDatabase();

    private readonly SemaphoreSlim _semaphore = new(1, 1);


    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null,
        bool useLocalCache = true)
    {
        // Try to get from cache first
        var cachedItem = await GetAsync<T>(key, useLocalCache);
        if (cachedItem != null)
        {
            return cachedItem;
        }

        // If not in cache, acquire a lock to prevent multiple factory executions
        await _semaphore.WaitAsync();
        try
        {
            // Check cache again in case it was populated while waiting for the lock
            cachedItem = await GetAsync<T>(key, useLocalCache);
            if (cachedItem != null)
            {
                return cachedItem;
            }

            // Execute the factory to get the value
            var value = await factory();
            if (value != null)
            {
                // Cache the value
                await SetAsync(key, value, expiration ?? GetExpirationForKey(key));
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or setting cache value for key {Key}", key);
            return default;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, bool useLocalCache = true)
    {
        try
        {
            // Try to get from memory cache first for better performance
            if (useLocalCache && _memoryCache.TryGetValue(key, out T? cachedValue))
            {
                return cachedValue;
            }

            // Get from distributed cache
            var cachedBytes = await _distributedCache.GetAsync(key);
            if (cachedBytes == null || cachedBytes.Length == 0)
            {
                return default;
            }

            // Deserialize the value
            var json = Encoding.UTF8.GetString(cachedBytes);
            var value = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            // Store in memory cache for future use
            if (useLocalCache && value != null)
            {
                // Create a cache entry with Size specified
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(GetLocalExpirationForKey(key))
                    .SetSize(1); // Default size of 1 for all entries

                _memoryCache.Set(key, value, cacheEntryOptions);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key {Key}", key);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? GetExpirationForKey(key)
            };

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var cachedBytes = Encoding.UTF8.GetBytes(json);

            // Set in distributed cache
            await _distributedCache.SetAsync(key, cachedBytes, options);

            // Set in memory cache for better performance with Size specified
            var memoryCacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration ?? GetLocalExpirationForKey(key))
                .SetSize(1); // Default size of 1 for all entries

            _memoryCache.Set(key, value, memoryCacheOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        try
        {
            // Remove from both caches
            await _distributedCache.RemoveAsync(key);
            _memoryCache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern)
    {
        // Efficiently implement pattern-based cache invalidation using Redis SCAN
        try
        {
            // If Redis is not available, we cannot enumerate keys. Best-effort log and return.
            if (_redis == null || _redisDb == null)
            {
                _logger.LogInformation("Redis not configured; skipping pattern removal for pattern {Pattern}", pattern);
                return;
            }

            // Special handling for high-traffic patterns
            if (pattern.StartsWith("feed:"))
            {
                _logger.LogInformation("Optimized cache invalidation for feed pattern: {Pattern}", pattern);

                // If it's the global feed, we need to invalidate common page combinations
                if (pattern.Contains("global"))
                {
                    // Invalidate first few pages with common page sizes
                    for (int page = 1; page <= 3; page++)
                    {
                        foreach (var size in new[] { 10, 20, 50 })
                        {
                            var specificKey = $"feed:global:{page}:{size}:all";
                            await _distributedCache.RemoveAsync(specificKey);
                            _memoryCache.Remove(specificKey);
                        }
                    }
                }
            }
            else if (pattern.StartsWith("post:"))
            {
                _logger.LogInformation("Optimized cache invalidation for post pattern: {Pattern}", pattern);

                // For post-specific patterns, we also need to invalidate feed caches
                // that might contain this post
                await this.RemoveByPatternAsync("feed:global:1:*");
            }

            // Use Redis SCAN to find and delete all matching keys
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                const int batchSize = 100;
                var keysToDelete = new List<RedisKey>(batchSize);

                await foreach (var key in server.KeysAsync(pattern: pattern))
                {
                    keysToDelete.Add(key);
                    // Also remove from local memory cache
                    _memoryCache.Remove(key.ToString().Replace("LiterateWinnerApi:", ""));

                    if (keysToDelete.Count >= batchSize)
                    {
                        await _redisDb.KeyDeleteAsync(keysToDelete.ToArray());
                        keysToDelete.Clear();
                    }
                }

                // Delete any remaining keys
                if (keysToDelete.Count > 0)
                {
                    await _redisDb.KeyDeleteAsync(keysToDelete.ToArray());
                }
            }

            _logger.LogInformation("Successfully removed cache keys matching pattern {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache values for pattern {Pattern}", pattern);
        }
    }

    /// <inheritdoc />
    public async Task<IAsyncDisposable?> AcquireLockAsync(string key, TimeSpan? expiration = null)
    {
        // Implement a simple distributed lock using Redis
        var lockKey = $"lock:{key}";
        var lockId = Guid.NewGuid().ToString();
        var expirationTime = expiration ?? TimeSpan.FromSeconds(30);

        // Try to acquire the lock
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };

        try
        {
            await _distributedCache.SetAsync(lockKey, Encoding.UTF8.GetBytes(lockId), options);

            // Return a disposable to release the lock when done
            return new AsyncDisposableLock(async () =>
            {
                try
                {
                    // Only release if it's our lock
                    var currentLockId = await _distributedCache.GetStringAsync(lockKey);
                    if (currentLockId == lockId)
                    {
                        await _distributedCache.RemoveAsync(lockKey);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error releasing lock for key {Key}", lockKey);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring lock for key {Key}", lockKey);
            return null;
        }
    }

    /// <summary>
    /// Get the appropriate expiration time for a key based on its pattern
    /// </summary>
    private TimeSpan GetExpirationForKey(string key)
    {
        // Use shorter cache times for comment-related data
        if (key.Contains("comments:") || key.Contains("post-guid:"))
        {
            return _commentOptions.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(2);
        }

        // Use default for everything else
        return _defaultOptions.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10);
    }

    /// <summary>
    /// Get the appropriate local cache expiration time for a key based on its pattern
    /// Always use shorter times for local memory cache
    /// </summary>
    private TimeSpan GetLocalExpirationForKey(string key)
    {
        // Use shorter cache times for comment-related data
        if (key.Contains("comments:") || key.Contains("post-guid:"))
        {
            return TimeSpan.FromMinutes(1);
        }

        // Use default for everything else
        return TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Helper class for implementing IAsyncDisposable for distributed locks
    /// </summary>
    private class AsyncDisposableLock : IAsyncDisposable
    {
        private readonly Func<Task> _releaseAction;

        public AsyncDisposableLock(Func<Task> releaseAction)
        {
            _releaseAction = releaseAction;
        }

        public async ValueTask DisposeAsync()
        {
            await _releaseAction();
        }
    }
}