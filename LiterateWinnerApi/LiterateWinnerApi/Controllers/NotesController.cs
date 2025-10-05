using JobApplicationTrackerApi.DTO.Notes;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
using JobApplicationTrackerApi.Services.IdentityService;
using JobApplicationTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Controllers;

/// <summary>
/// Controller for managing application notes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController(
    DefaultContext context,
    IIdentityService identityService,
    ILogger<NotesController> logger
) : ControllerBase
{
    private readonly DefaultContext _context = context ?? throw new ArgumentNullException(nameof(context));
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

            // Verify the application belongs to the user
            var applicationExists = await _context.Applications
                .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == Enum.CommonStatus.Active);

            if (!applicationExists)
            {
                return NotFound(ApiResponse<object>.Failure("Application not found"));
            }

            var notes = await _context.Notes
                .Where(n => n.ApplicationId == applicationId && n.Status == Enum.CommonStatus.Active)
                .OrderByDescending(n => n.CreatedUtc)
                .Select(n => new NoteResponseDto
                {
                    Id = n.Id,
                    ApplicationId = n.ApplicationId,
                    Title = n.Title,
                    Content = n.Content,
                    NoteType = n.NoteType,
                    CreatedUtc = n.CreatedUtc,
                    CreatedBy = n.CreatedBy,
                    UpdatedUtc = n.UpdatedUtc,
                    UpdatedBy = n.UpdatedBy
                })
                .ToListAsync();

            return Ok(ApiResponse<List<NoteResponseDto>>.Success(notes, "Notes retrieved successfully"));
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

            var note = await _context.Notes
                .Include(n => n.Application)
                .Where(n => n.Id == id && n.Status == Enum.CommonStatus.Active && n.Application.UserId == userId)
                .Select(n => new NoteResponseDto
                {
                    Id = n.Id,
                    ApplicationId = n.ApplicationId,
                    Title = n.Title,
                    Content = n.Content,
                    NoteType = n.NoteType,
                    CreatedUtc = n.CreatedUtc,
                    CreatedBy = n.CreatedBy,
                    UpdatedUtc = n.UpdatedUtc,
                    UpdatedBy = n.UpdatedBy
                })
                .FirstOrDefaultAsync();

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

            // Verify the application belongs to the user
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == createNoteDto.ApplicationId && a.UserId == userId && a.Status == Enum.CommonStatus.Active);

            if (application == null)
            {
                return NotFound(ApiResponse<object>.Failure("Application not found"));
            }

            var note = new Note
            {
                ApplicationId = createNoteDto.ApplicationId,
                Title = createNoteDto.Title,
                Content = createNoteDto.Content,
                NoteType = createNoteDto.NoteType,
                CreatedBy = userId,
                Status = Enum.CommonStatus.Active
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var responseDto = new NoteResponseDto
            {
                Id = note.Id,
                ApplicationId = note.ApplicationId,
                Title = note.Title,
                Content = note.Content,
                NoteType = note.NoteType,
                CreatedUtc = note.CreatedUtc,
                CreatedBy = note.CreatedBy,
                UpdatedUtc = note.UpdatedUtc,
                UpdatedBy = note.UpdatedBy
            };

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, 
                ApiResponse<NoteResponseDto>.Success(responseDto, "Note created successfully"));
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

            var note = await _context.Notes
                .Include(n => n.Application)
                .FirstOrDefaultAsync(n => n.Id == id && n.Status == Enum.CommonStatus.Active && n.Application.UserId == userId);

            if (note == null)
            {
                return NotFound(ApiResponse<object>.Failure("Note not found"));
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateNoteDto.Title))
                note.Title = updateNoteDto.Title;

            if (!string.IsNullOrEmpty(updateNoteDto.Content))
                note.Content = updateNoteDto.Content;

            if (updateNoteDto.NoteType.HasValue)
                note.NoteType = updateNoteDto.NoteType.Value;

            note.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            var responseDto = new NoteResponseDto
            {
                Id = note.Id,
                ApplicationId = note.ApplicationId,
                Title = note.Title,
                Content = note.Content,
                NoteType = note.NoteType,
                CreatedUtc = note.CreatedUtc,
                CreatedBy = note.CreatedBy,
                UpdatedUtc = note.UpdatedUtc,
                UpdatedBy = note.UpdatedBy
            };

            return Ok(ApiResponse<NoteResponseDto>.Success(responseDto, "Note updated successfully"));
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

            var note = await _context.Notes
                .Include(n => n.Application)
                .FirstOrDefaultAsync(n => n.Id == id && n.Status == Enum.CommonStatus.Active && n.Application.UserId == userId);

            if (note == null)
            {
                return NotFound(ApiResponse<object>.Failure("Note not found"));
            }

            // Soft delete
            note.Status = Enum.CommonStatus.Delete;
            note.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(null, "Note deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while deleting the note"));
        }
    }
}
