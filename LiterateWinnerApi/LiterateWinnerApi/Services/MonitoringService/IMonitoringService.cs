namespace JobApplicationTrackerApi.Services.MonitoringService;

/// <summary>
/// Interface for monitoring service
/// </summary>
public interface IMonitoringService
{
    /// <summary>
    /// Record API request start
    /// </summary>
    void RecordRequestStart(string endpoint, string method);

    /// <summary>
    /// Record API request completion with timing
    /// </summary>
    void RecordRequestEnd(string endpoint, string method, double elapsedMs, bool isSuccess);

    /// <summary>
    /// Record database query execution time
    /// </summary>
    void RecordDbQueryTime(string queryName, double elapsedMs);

    /// <summary>
    /// Record cache hit
    /// </summary>
    void RecordCacheHit(string cacheKey, double elapsedMs);

    /// <summary>
    /// Record cache miss
    /// </summary>
    void RecordCacheMiss(string cacheKey, double elapsedMs);

    /// <summary>
    /// Record session start
    /// </summary>
    void RecordSessionStart(string userId);

    /// <summary>
    /// Record session end
    /// </summary>
    void RecordSessionEnd(string userId);

    /// <summary>
    /// Record API rate limit hit
    /// </summary>
    void RecordRateLimitHit(string endpoint, string clientIp);

    /// <summary>
    /// Get monitoring statistics for dashboard
    /// </summary>
    Task<MonitoringStatistics> GetStatisticsAsync();

    /// <summary>
    /// Reset counters for new monitoring period
    /// </summary>
    void ResetCounters();
}