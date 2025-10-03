namespace JobApplicationTrackerApi.Services.MonitoringService;

/// <summary>
/// Contains application monitoring statistics
/// </summary>
public class MonitoringStatistics
{
    /// <summary>
    /// Number of active user sessions
    /// </summary>
    public long ActiveSessions { get; set; }

    /// <summary>
    /// Total number of API requests processed
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// Error rate (0.0 to 1.0)
    /// </summary>
    public double ErrorRate { get; set; }

    /// <summary>
    /// Average API response time in milliseconds
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Cache hit rate (0.0 to 1.0)
    /// </summary>
    public double CacheHitRate { get; set; }

    /// <summary>
    /// Number of active database connections
    /// </summary>
    public int ActiveDbConnections { get; set; }

    /// <summary>
    /// Maximum allowed database connections
    /// </summary>
    public int MaxDbConnections { get; set; }

    /// <summary>
    /// CPU usage percentage
    /// </summary>
    public double CpuUsagePercent { get; set; }

    /// <summary>
    /// Memory usage in megabytes
    /// </summary>
    public double MemoryUsageMb { get; set; }

    /// <summary>
    /// Number of active threads
    /// </summary>
    public long ThreadCount { get; set; }

    /// <summary>
    /// Application uptime in seconds
    /// </summary>
    public double UptimeSeconds { get; set; }

    /// <summary>
    /// Total registered users
    /// </summary>
    public int TotalUsers { get; set; }
}