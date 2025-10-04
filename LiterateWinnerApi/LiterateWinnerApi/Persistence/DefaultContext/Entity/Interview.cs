using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents an interview scheduled for a job application
/// </summary>
public class Interview : BaseEntity
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
    /// Date and time of the interview
    /// </summary>
    public DateTime InterviewDate { get; set; }

    /// <summary>
    /// Type of interview
    /// </summary>
    public InterviewType InterviewType { get; set; }

    /// <summary>
    /// Duration of the interview in minutes
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Name of the interviewer
    /// </summary>
    public string? InterviewerName { get; set; }

    /// <summary>
    /// Position/title of the interviewer
    /// </summary>
    public string? InterviewerPosition { get; set; }

    /// <summary>
    /// Location of the interview (office address or "Virtual")
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Meeting link for virtual interviews (Zoom, Teams, etc.)
    /// </summary>
    public string? MeetingLink { get; set; }

    /// <summary>
    /// Interview notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Outcome of the interview
    /// </summary>
    public string? Outcome { get; set; }

    /// <summary>
    /// Whether email reminder has been sent
    /// </summary>
    public bool ReminderSent { get; set; } = false;

    /// <summary>
    /// Navigation property to Application
    /// </summary>
    public virtual Application Application { get; set; } = null!;
}