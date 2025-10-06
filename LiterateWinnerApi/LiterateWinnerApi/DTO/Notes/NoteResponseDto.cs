using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.DTO.Notes;

/// <summary>
/// DTO for note response
/// </summary>
public sealed record NoteResponseDto
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
    /// Title of the note
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Content of the note (supports rich text/Markdown)
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Type of the note
    /// </summary>
    public NoteType NoteType { get; init; }

    /// <summary>
    /// Date and time when the note was created
    /// </summary>
    public DateTime CreatedUtc { get; init; }

    /// <summary>
    /// User who created the note
    /// </summary>
    public string CreatedBy { get; init; } = string.Empty;

    /// <summary>
    /// Date and time when the note was last updated
    /// </summary>
    public DateTime? UpdatedUtc { get; init; }

    /// <summary>
    /// User who last updated the note
    /// </summary>
    public string? UpdatedBy { get; init; }
}
