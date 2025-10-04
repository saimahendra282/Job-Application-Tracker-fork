using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents a job application
/// </summary>
public class Application : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the company
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Position title
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Job location (City, State/Country)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// URL to the job posting
    /// </summary>
    public string? JobUrl { get; set; }

    /// <summary>
    /// Current status of the application
    /// </summary>
    public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Applied;

    /// <summary>
    /// Priority level of the application
    /// </summary>
    public ApplicationPriority Priority { get; set; } = ApplicationPriority.Medium;

    /// <summary>
    /// Expected or offered salary
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Minimum salary in range
    /// </summary>
    public decimal? SalaryMin { get; set; }

    /// <summary>
    /// Maximum salary in range
    /// </summary>
    public decimal? SalaryMax { get; set; }

    /// <summary>
    /// Date when the application was submitted
    /// </summary>
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the company responded
    /// </summary>
    public DateTime? ResponseDate { get; set; }

    /// <summary>
    /// Date when the offer was received
    /// </summary>
    public DateTime? OfferDate { get; set; }

    /// <summary>
    /// Job description text
    /// </summary>
    public string? JobDescription { get; set; }

    /// <summary>
    /// Job requirements text
    /// </summary>
    public string? Requirements { get; set; }

    /// <summary>
    /// Foreign key to ApplicationUser (from IdentityContext)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to Interviews
    /// </summary>
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    /// <summary>
    /// Navigation property to Notes
    /// </summary>
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    /// <summary>
    /// Navigation property to Contacts
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    /// <summary>
    /// Navigation property to Documents
    /// </summary>
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Navigation property to StatusHistory
    /// </summary>
    public virtual ICollection<StatusHistory> StatusHistory { get; set; } = new List<StatusHistory>();
}