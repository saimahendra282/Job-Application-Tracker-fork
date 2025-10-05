using System.ComponentModel.DataAnnotations;
using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.DTO.Notes;

/// <summary>
/// DTO for creating a new note
/// </summary>
public sealed record CreateNoteDto
{
    /// <summary>
    /// Foreign key to Application
    /// </summary>
    [Required]
    public int ApplicationId { get; init; }

    /// <summary>
    /// Title of the note
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Content of the note (supports rich text/Markdown)
    /// </summary>
    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Type of the note
    /// </summary>
    [Required]
    public NoteType NoteType { get; init; } = NoteType.General;
}
