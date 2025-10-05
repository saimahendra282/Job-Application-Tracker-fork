using JobApplicationTrackerApi.DTO.Notes;
using JobApplicationTrackerApi.Enum;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
using JobApplicationTrackerApi.Services.CacheService;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Services.NotesService;

/// <summary>
/// Service for managing application notes
/// </summary>
public class NotesService(
    DefaultContext context,
    ICacheService cacheService,
    ILogger<NotesService> logger
) : INotesService
{
    private readonly DefaultContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly ILogger<NotesService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<NoteResponseDto>> GetNotesByApplicationAsync(int applicationId, string userId)
    {
        try
        {
            var cacheKey = $"notes_application_{applicationId}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                // Verify the application belongs to the user
                var applicationExists = await _context.Applications
                    .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == CommonStatus.Active);

                if (!applicationExists)
                {
                    throw new InvalidOperationException("Application not found or access denied");
                }

                var notes = await _context.Notes
                    .Where(n => n.ApplicationId == applicationId && n.Status == CommonStatus.Active)
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

                return notes;
            }, TimeSpan.FromMinutes(5)) ?? new List<NoteResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes for application {ApplicationId} and user {UserId}", applicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NoteResponseDto?> GetNoteByIdAsync(int id, string userId)
    {
        try
        {
            var cacheKey = $"note_{id}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var note = await _context.Notes
                    .Include(n => n.Application)
                    .Where(n => n.Id == id && n.Status == CommonStatus.Active && n.Application.UserId == userId)
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

                return note;
            }, TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {NoteId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NoteResponseDto> CreateNoteAsync(CreateNoteDto createNoteDto, string userId)
    {
        try
        {
            // Verify the application belongs to the user
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == createNoteDto.ApplicationId && a.UserId == userId && a.Status == CommonStatus.Active);

            if (application == null)
            {
                throw new InvalidOperationException("Application not found or access denied");
            }

            var note = new Note
            {
                ApplicationId = createNoteDto.ApplicationId,
                Title = createNoteDto.Title,
                Content = createNoteDto.Content,
                NoteType = createNoteDto.NoteType,
                CreatedBy = userId,
                Status = CommonStatus.Active
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"notes_application_{createNoteDto.ApplicationId}_{userId}");

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

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note for application {ApplicationId} and user {UserId}", createNoteDto.ApplicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NoteResponseDto> UpdateNoteAsync(int id, UpdateNoteDto updateNoteDto, string userId)
    {
        try
        {
            var note = await _context.Notes
                .Include(n => n.Application)
                .FirstOrDefaultAsync(n => n.Id == id && n.Status == CommonStatus.Active && n.Application.UserId == userId);

            if (note == null)
            {
                throw new InvalidOperationException("Note not found or access denied");
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

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"notes_application_{note.ApplicationId}_{userId}");
            await _cacheService.RemoveAsync($"note_{id}_{userId}");

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

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteNoteAsync(int id, string userId)
    {
        try
        {
            var note = await _context.Notes
                .Include(n => n.Application)
                .FirstOrDefaultAsync(n => n.Id == id && n.Status == CommonStatus.Active && n.Application.UserId == userId);

            if (note == null)
            {
                throw new InvalidOperationException("Note not found or access denied");
            }

            // Soft delete
            note.Status = CommonStatus.Delete;
            note.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"notes_application_{note.ApplicationId}_{userId}");
            await _cacheService.RemoveAsync($"note_{id}_{userId}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<NoteResponseDto>> GetNotesByTypeAsync(int applicationId, NoteType noteType, string userId)
    {
        try
        {
            var cacheKey = $"notes_application_{applicationId}_type_{noteType}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                // Verify the application belongs to the user
                var applicationExists = await _context.Applications
                    .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == CommonStatus.Active);

                if (!applicationExists)
                {
                    throw new InvalidOperationException("Application not found or access denied");
                }

                var notes = await _context.Notes
                    .Where(n => n.ApplicationId == applicationId && n.NoteType == noteType && n.Status == CommonStatus.Active)
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

                return notes;
            }, TimeSpan.FromMinutes(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes by type {NoteType} for application {ApplicationId} and user {UserId}", noteType, applicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<NoteResponseDto>> SearchNotesAsync(int applicationId, string searchTerm, string userId)
    {
        try
        {
            // Verify the application belongs to the user
            var applicationExists = await _context.Applications
                .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == CommonStatus.Active);

            if (!applicationExists)
            {
                throw new InvalidOperationException("Application not found or access denied");
            }

            var notes = await _context.Notes
                .Where(n => n.ApplicationId == applicationId && 
                           n.Status == CommonStatus.Active &&
                           (n.Title.Contains(searchTerm) || n.Content.Contains(searchTerm)))
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

            return notes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching notes for application {ApplicationId} with term '{SearchTerm}' and user {UserId}", applicationId, searchTerm, userId);
            throw;
        }
    }
}
