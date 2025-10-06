namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents a contact person associated with a job application
/// </summary>
public class Contact : BaseEntity
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
    /// Name of the contact person
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Job title/position of the contact
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Email address of the contact
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number of the contact
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// LinkedIn profile URL
    /// </summary>
    public string? LinkedIn { get; set; }

    /// <summary>
    /// Additional notes about the contact
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this is the primary contact for the application
    /// </summary>
    public bool IsPrimaryContact { get; set; } = false;

    /// <summary>
    /// Navigation property to Application
    /// </summary>
    public virtual Application Application { get; set; } = null!;
}