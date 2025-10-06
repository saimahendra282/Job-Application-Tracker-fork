using System.ComponentModel.DataAnnotations;
using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.DTO.Notes;

/// <summary>
/// DTO for updating an existing note
/// </summary>
public sealed record UpdateNoteDto
{
    /// <summary>
    /// Title of the note
    /// </summary>
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; init; }

    /// <summary>
    /// Content of the note (supports rich text/Markdown)
    /// </summary>
    [StringLength(10000, MinimumLength = 1)]
    public string? Content { get; init; }

    /// <summary>
    /// Type of the note
    /// </summary>
    public NoteType? NoteType { get; init; }
}
