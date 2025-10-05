namespace JobApplicationTrackerApi.DTO.Contacts;

/// <summary>
/// DTO for contact response
/// </summary>
public sealed record ContactResponseDto
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Foreign key to Application
    /// </summary>
    public int ApplicationId { get; init; }

    /// <summary>
    /// Name of the contact person
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Job title/position of the contact
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Email address of the contact
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Phone number of the contact
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// LinkedIn profile URL
    /// </summary>
    public string? LinkedIn { get; init; }

    /// <summary>
    /// Additional notes about the contact
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Whether this is the primary contact for the application
    /// </summary>
    public bool IsPrimaryContact { get; init; }

    /// <summary>
    /// Date and time when the contact was created
    /// </summary>
    public DateTime CreatedUtc { get; init; }

    /// <summary>
    /// User who created the contact
    /// </summary>
    public string CreatedBy { get; init; } = string.Empty;

    /// <summary>
    /// Date and time when the contact was last updated
    /// </summary>
    public DateTime? UpdatedUtc { get; init; }

    /// <summary>
    /// User who last updated the contact
    /// </summary>
    public string? UpdatedBy { get; init; }
}
