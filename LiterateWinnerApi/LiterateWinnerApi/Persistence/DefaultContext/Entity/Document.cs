using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents a document associated with a job application
/// </summary>
public class Document : BaseEntity
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
    /// Type of the document
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// URL or path to the stored file
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Navigation property to Application
    /// </summary>
    public virtual Application Application { get; set; } = null!;
}