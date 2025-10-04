namespace JobApplicationTrackerApi.Enum;

/// <summary>
/// Represents the type of interview
/// </summary>
public enum InterviewType
{
    /// <summary>
    /// Phone interview
    /// </summary>
    Phone = 1,

    /// <summary>
    /// Video interview (Zoom, Teams, etc.)
    /// </summary>
    Video = 2,

    /// <summary>
    /// On-site interview
    /// </summary>
    Onsite = 3,

    /// <summary>
    /// Technical interview
    /// </summary>
    Technical = 4,

    /// <summary>
    /// HR interview
    /// </summary>
    HR = 5,

    /// <summary>
    /// Final interview
    /// </summary>
    Final = 6
}