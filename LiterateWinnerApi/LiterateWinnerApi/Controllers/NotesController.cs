using JobApplicationTrackerApi.DTO.Notes;
using JobApplicationTrackerApi.Services.NotesService;
using JobApplicationTrackerApi.Services.IdentityService;
using JobApplicationTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTrackerApi.Controllers;

/// <summary>
/// Controller for managing application notes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController(
    INotesService notesService,
    IIdentityService identityService,
    ILogger<NotesController> logger
) : ControllerBase
{
    private readonly INotesService _notesService = notesService ?? throw new ArgumentNullException(nameof(notesService));
    private readonly IIdentityService _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    private readonly ILogger<NotesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Get all notes for a specific application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>List of notes for the application</returns>
    [HttpGet("application/{applicationId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<NoteResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNotesByApplication(int applicationId)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var notes = await _notesService.GetNotesByApplicationAsync(applicationId, userId);
            return Ok(ApiResponse<List<NoteResponseDto>>.Success(notes, "Notes retrieved successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Application not found or access denied for application {ApplicationId}", applicationId);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes for application {ApplicationId}", applicationId);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while retrieving notes"));
        }
    }

    /// <summary>
    /// Get a specific note by ID
    /// </summary>
    /// <param name="id">The note ID</param>
    /// <returns>The note details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNote(int id)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var note = await _notesService.GetNoteByIdAsync(id, userId);
            if (note == null)
            {
                return NotFound(ApiResponse<object>.Failure("Note not found"));
            }

            return Ok(ApiResponse<NoteResponseDto>.Success(note, "Note retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {NoteId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while retrieving the note"));
        }
    }

    /// <summary>
    /// Create a new note
    /// </summary>
    /// <param name="createNoteDto">The note data</param>
    /// <returns>The created note</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto createNoteDto)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var note = await _notesService.CreateNoteAsync(createNoteDto, userId);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, 
                ApiResponse<NoteResponseDto>.Success(note, "Note created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Application not found or access denied for application {ApplicationId}", createNoteDto.ApplicationId);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note for application {ApplicationId}", createNoteDto.ApplicationId);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while creating the note"));
        }
    }

    /// <summary>
    /// Update an existing note
    /// </summary>
    /// <param name="id">The note ID</param>
    /// <param name="updateNoteDto">The updated note data</param>
    /// <returns>The updated note</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto updateNoteDto)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var note = await _notesService.UpdateNoteAsync(id, updateNoteDto, userId);
            return Ok(ApiResponse<NoteResponseDto>.Success(note, "Note updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Note not found or access denied for note {NoteId}", id);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while updating the note"));
        }
    }

    /// <summary>
    /// Delete a note
    /// </summary>
    /// <param name="id">The note ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteNote(int id)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var result = await _notesService.DeleteNoteAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Failure("Note not found"));
            }

            return Ok(ApiResponse<object>.Success(new object(), "Note deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Note not found or access denied for note {NoteId}", id);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while deleting the note"));
        }
    }
}
