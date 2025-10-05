using JobApplicationTrackerApi.DTO.Contacts;
using JobApplicationTrackerApi.Enum;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
using JobApplicationTrackerApi.Services.CacheService;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Services.ContactsService;

/// <summary>
/// Service for managing application contacts
/// </summary>
public class ContactsService(
    DefaultContext context,
    ICacheService cacheService,
    ILogger<ContactsService> logger
) : IContactsService
{
    private readonly DefaultContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly ILogger<ContactsService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<ContactResponseDto>> GetContactsByApplicationAsync(int applicationId, string userId)
    {
        try
        {
            var cacheKey = $"contacts_application_{applicationId}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                // Verify the application belongs to the user
                var applicationExists = await _context.Applications
                    .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == CommonStatus.Active);

                if (!applicationExists)
                {
                    throw new InvalidOperationException("Application not found or access denied");
                }

                var contacts = await _context.Contacts
                    .Where(c => c.ApplicationId == applicationId && c.Status == CommonStatus.Active)
                    .OrderBy(c => c.IsPrimaryContact ? 0 : 1)
                    .ThenBy(c => c.Name)
                    .Select(c => new ContactResponseDto
                    {
                        Id = c.Id,
                        ApplicationId = c.ApplicationId,
                        Name = c.Name,
                        Position = c.Position,
                        Email = c.Email,
                        Phone = c.Phone,
                        LinkedIn = c.LinkedIn,
                        Notes = c.Notes,
                        IsPrimaryContact = c.IsPrimaryContact,
                        CreatedUtc = c.CreatedUtc,
                        CreatedBy = c.CreatedBy,
                        UpdatedUtc = c.UpdatedUtc,
                        UpdatedBy = c.UpdatedBy
                    })
                    .ToListAsync();

                return contacts;
            }, TimeSpan.FromMinutes(5)) ?? new List<ContactResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contacts for application {ApplicationId} and user {UserId}", applicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ContactResponseDto?> GetContactByIdAsync(int id, string userId)
    {
        try
        {
            var cacheKey = $"contact_{id}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var contact = await _context.Contacts
                    .Include(c => c.Application)
                    .Where(c => c.Id == id && c.Status == CommonStatus.Active && c.Application.UserId == userId)
                    .Select(c => new ContactResponseDto
                    {
                        Id = c.Id,
                        ApplicationId = c.ApplicationId,
                        Name = c.Name,
                        Position = c.Position,
                        Email = c.Email,
                        Phone = c.Phone,
                        LinkedIn = c.LinkedIn,
                        Notes = c.Notes,
                        IsPrimaryContact = c.IsPrimaryContact,
                        CreatedUtc = c.CreatedUtc,
                        CreatedBy = c.CreatedBy,
                        UpdatedUtc = c.UpdatedUtc,
                        UpdatedBy = c.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                return contact;
            }, TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact {ContactId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ContactResponseDto> CreateContactAsync(CreateContactDto createContactDto, string userId)
    {
        try
        {
            // Verify the application belongs to the user
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == createContactDto.ApplicationId && a.UserId == userId && a.Status == CommonStatus.Active);

            if (application == null)
            {
                throw new InvalidOperationException("Application not found or access denied");
            }

            // If this contact is marked as primary, unmark other primary contacts for this application
            if (createContactDto.IsPrimaryContact)
            {
                var existingPrimaryContacts = await _context.Contacts
                    .Where(c => c.ApplicationId == createContactDto.ApplicationId && c.IsPrimaryContact && c.Status == CommonStatus.Active)
                    .ToListAsync();

                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimaryContact = false;
                    existingContact.UpdatedBy = userId;
                }
            }

            var contact = new Contact
            {
                ApplicationId = createContactDto.ApplicationId,
                Name = createContactDto.Name,
                Position = createContactDto.Position,
                Email = createContactDto.Email,
                Phone = createContactDto.Phone,
                LinkedIn = createContactDto.LinkedIn,
                Notes = createContactDto.Notes,
                IsPrimaryContact = createContactDto.IsPrimaryContact,
                CreatedBy = userId,
                Status = CommonStatus.Active
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"contacts_application_{createContactDto.ApplicationId}_{userId}");

            var responseDto = new ContactResponseDto
            {
                Id = contact.Id,
                ApplicationId = contact.ApplicationId,
                Name = contact.Name,
                Position = contact.Position,
                Email = contact.Email,
                Phone = contact.Phone,
                LinkedIn = contact.LinkedIn,
                Notes = contact.Notes,
                IsPrimaryContact = contact.IsPrimaryContact,
                CreatedUtc = contact.CreatedUtc,
                CreatedBy = contact.CreatedBy,
                UpdatedUtc = contact.UpdatedUtc,
                UpdatedBy = contact.UpdatedBy
            };

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact for application {ApplicationId} and user {UserId}", createContactDto.ApplicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ContactResponseDto> UpdateContactAsync(int id, UpdateContactDto updateContactDto, string userId)
    {
        try
        {
            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                throw new InvalidOperationException("Contact not found or access denied");
            }

            // If this contact is being marked as primary, unmark other primary contacts for this application
            if (updateContactDto.IsPrimaryContact == true)
            {
                var existingPrimaryContacts = await _context.Contacts
                    .Where(c => c.ApplicationId == contact.ApplicationId && c.IsPrimaryContact && c.Id != id && c.Status == CommonStatus.Active)
                    .ToListAsync();

                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimaryContact = false;
                    existingContact.UpdatedBy = userId;
                }
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateContactDto.Name))
                contact.Name = updateContactDto.Name;

            if (updateContactDto.Position != null)
                contact.Position = updateContactDto.Position;

            if (updateContactDto.Email != null)
                contact.Email = updateContactDto.Email;

            if (updateContactDto.Phone != null)
                contact.Phone = updateContactDto.Phone;

            if (updateContactDto.LinkedIn != null)
                contact.LinkedIn = updateContactDto.LinkedIn;

            if (updateContactDto.Notes != null)
                contact.Notes = updateContactDto.Notes;

            if (updateContactDto.IsPrimaryContact.HasValue)
                contact.IsPrimaryContact = updateContactDto.IsPrimaryContact.Value;

            contact.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"contacts_application_{contact.ApplicationId}_{userId}");
            await _cacheService.RemoveAsync($"contact_{id}_{userId}");

            var responseDto = new ContactResponseDto
            {
                Id = contact.Id,
                ApplicationId = contact.ApplicationId,
                Name = contact.Name,
                Position = contact.Position,
                Email = contact.Email,
                Phone = contact.Phone,
                LinkedIn = contact.LinkedIn,
                Notes = contact.Notes,
                IsPrimaryContact = contact.IsPrimaryContact,
                CreatedUtc = contact.CreatedUtc,
                CreatedBy = contact.CreatedBy,
                UpdatedUtc = contact.UpdatedUtc,
                UpdatedBy = contact.UpdatedBy
            };

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {ContactId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteContactAsync(int id, string userId)
    {
        try
        {
            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                throw new InvalidOperationException("Contact not found or access denied");
            }

            // Soft delete
            contact.Status = CommonStatus.Delete;
            contact.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"contacts_application_{contact.ApplicationId}_{userId}");
            await _cacheService.RemoveAsync($"contact_{id}_{userId}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {ContactId} for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetPrimaryContactAsync(int id, string userId)
    {
        try
        {
            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                throw new InvalidOperationException("Contact not found or access denied");
            }

            // Unmark all other primary contacts for this application
            var existingPrimaryContacts = await _context.Contacts
                .Where(c => c.ApplicationId == contact.ApplicationId && c.IsPrimaryContact && c.Id != id && c.Status == CommonStatus.Active)
                .ToListAsync();

            foreach (var existingContact in existingPrimaryContacts)
            {
                existingContact.IsPrimaryContact = false;
                existingContact.UpdatedBy = userId;
            }

            // Set this contact as primary
            contact.IsPrimaryContact = true;
            contact.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // Clear related cache entries
            await _cacheService.RemoveByPatternAsync($"contacts_application_{contact.ApplicationId}_{userId}");
            await _cacheService.RemoveAsync($"contact_{id}_{userId}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting contact {ContactId} as primary for user {UserId}", id, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ContactResponseDto?> GetPrimaryContactAsync(int applicationId, string userId)
    {
        try
        {
            var cacheKey = $"primary_contact_application_{applicationId}_{userId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                // Verify the application belongs to the user
                var applicationExists = await _context.Applications
                    .AnyAsync(a => a.Id == applicationId && a.UserId == userId && a.Status == CommonStatus.Active);

                if (!applicationExists)
                {
                    throw new InvalidOperationException("Application not found or access denied");
                }

                var contact = await _context.Contacts
                    .Where(c => c.ApplicationId == applicationId && c.IsPrimaryContact && c.Status == CommonStatus.Active)
                    .Select(c => new ContactResponseDto
                    {
                        Id = c.Id,
                        ApplicationId = c.ApplicationId,
                        Name = c.Name,
                        Position = c.Position,
                        Email = c.Email,
                        Phone = c.Phone,
                        LinkedIn = c.LinkedIn,
                        Notes = c.Notes,
                        IsPrimaryContact = c.IsPrimaryContact,
                        CreatedUtc = c.CreatedUtc,
                        CreatedBy = c.CreatedBy,
                        UpdatedUtc = c.UpdatedUtc,
                        UpdatedBy = c.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                return contact;
            }, TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving primary contact for application {ApplicationId} and user {UserId}", applicationId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<ContactResponseDto>> SearchContactsAsync(int applicationId, string searchTerm, string userId)
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

            var contacts = await _context.Contacts
                .Where(c => c.ApplicationId == applicationId && 
                           c.Status == CommonStatus.Active &&
                           (c.Name.Contains(searchTerm) || 
                            (c.Email != null && c.Email.Contains(searchTerm)) ||
                            (c.Position != null && c.Position.Contains(searchTerm))))
                .OrderBy(c => c.IsPrimaryContact ? 0 : 1)
                .ThenBy(c => c.Name)
                .Select(c => new ContactResponseDto
                {
                    Id = c.Id,
                    ApplicationId = c.ApplicationId,
                    Name = c.Name,
                    Position = c.Position,
                    Email = c.Email,
                    Phone = c.Phone,
                    LinkedIn = c.LinkedIn,
                    Notes = c.Notes,
                    IsPrimaryContact = c.IsPrimaryContact,
                    CreatedUtc = c.CreatedUtc,
                    CreatedBy = c.CreatedBy,
                    UpdatedUtc = c.UpdatedUtc,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync();

            return contacts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching contacts for application {ApplicationId} with term '{SearchTerm}' and user {UserId}", applicationId, searchTerm, userId);
            throw;
        }
    }
}
