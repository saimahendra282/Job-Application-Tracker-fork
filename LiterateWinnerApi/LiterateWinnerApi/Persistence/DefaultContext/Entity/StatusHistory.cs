using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents the history of status changes for a job application
/// </summary>
public class StatusHistory : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to Application
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// Previous status
    /// </summary>
    public ApplicationStatus? OldStatus { get; set; }

    /// <summary>
    /// New status
    /// </summary>
    public ApplicationStatus NewStatus { get; set; }

    /// <summary>
    /// Date and time when the status was changed
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Notes about the reason for the change
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to Application
    /// </summary>
    public virtual Application Application { get; set; } = null!;
}