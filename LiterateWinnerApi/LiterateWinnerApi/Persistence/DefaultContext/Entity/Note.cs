using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

/// <summary>
/// Represents a note associated with a job application
/// </summary>
public class Note : BaseEntity
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
    /// Title of the note
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the note (supports rich text/Markdown)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Type of the note
    /// </summary>
    public NoteType NoteType { get; set; } = NoteType.General;

    /// <summary>
    /// Navigation property to Application
    /// </summary>
    public virtual Application Application { get; set; } = null!;
}