namespace JobApplicationTrackerApi.Enum;

/// <summary>
/// Represents the status of a job application
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Application has been submitted
    /// </summary>
    Applied = 1,

    /// <summary>
    /// Application is in interview stage
    /// </summary>
    Interview = 2,

    /// <summary>
    /// Job offer has been received
    /// </summary>
    Offer = 3,

    /// <summary>
    /// Application has been rejected
    /// </summary>
    Rejected = 4
}