using JobApplicationTrackerApi.DTO.Contacts;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
using JobApplicationTrackerApi.Services.IdentityService;
using JobApplicationTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Controllers;

/// <summary>
/// Controller for managing application contacts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController(
    DefaultContext context,
    IIdentityService identityService,
    ILogger<ContactsController> logger
) : ControllerBase
{
    private readonly DefaultContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IIdentityService _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    private readonly ILogger<ContactsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Get all contacts for a specific application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>List of contacts for the application</returns>
    [HttpGet("application/{applicationId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<ContactResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContactsByApplication(int applicationId)
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

            var contacts = await _context.Contacts
                .Where(c => c.ApplicationId == applicationId && c.Status == Enum.CommonStatus.Active)
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

            return Ok(ApiResponse<List<ContactResponseDto>>.Success(contacts, "Contacts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contacts for application {ApplicationId}", applicationId);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while retrieving contacts"));
        }
    }

    /// <summary>
    /// Get a specific contact by ID
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <returns>The contact details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ContactResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContact(int id)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var contact = await _context.Contacts
                .Include(c => c.Application)
                .Where(c => c.Id == id && c.Status == Enum.CommonStatus.Active && c.Application.UserId == userId)
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

            if (contact == null)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            return Ok(ApiResponse<ContactResponseDto>.Success(contact, "Contact retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact {ContactId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while retrieving the contact"));
        }
    }

    /// <summary>
    /// Create a new contact
    /// </summary>
    /// <param name="createContactDto">The contact data</param>
    /// <returns>The created contact</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContactResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactDto createContactDto)
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
                .FirstOrDefaultAsync(a => a.Id == createContactDto.ApplicationId && a.UserId == userId && a.Status == Enum.CommonStatus.Active);

            if (application == null)
            {
                return NotFound(ApiResponse<object>.Failure("Application not found"));
            }

            // If this contact is marked as primary, unmark other primary contacts for this application
            if (createContactDto.IsPrimaryContact)
            {
                var existingPrimaryContacts = await _context.Contacts
                    .Where(c => c.ApplicationId == createContactDto.ApplicationId && c.IsPrimaryContact && c.Status == Enum.CommonStatus.Active)
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
                Status = Enum.CommonStatus.Active
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

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

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, 
                ApiResponse<ContactResponseDto>.Success(responseDto, "Contact created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact for application {ApplicationId}", createContactDto.ApplicationId);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while creating the contact"));
        }
    }

    /// <summary>
    /// Update an existing contact
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="updateContactDto">The updated contact data</param>
    /// <returns>The updated contact</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ContactResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactDto updateContactDto)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == Enum.CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            // If this contact is being marked as primary, unmark other primary contacts for this application
            if (updateContactDto.IsPrimaryContact == true)
            {
                var existingPrimaryContacts = await _context.Contacts
                    .Where(c => c.ApplicationId == contact.ApplicationId && c.IsPrimaryContact && c.Id != id && c.Status == Enum.CommonStatus.Active)
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

            return Ok(ApiResponse<ContactResponseDto>.Success(responseDto, "Contact updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {ContactId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while updating the contact"));
        }
    }

    /// <summary>
    /// Delete a contact
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteContact(int id)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == Enum.CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            // Soft delete
            contact.Status = Enum.CommonStatus.Delete;
            contact.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new object(), "Contact deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {ContactId}", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while deleting the contact"));
        }
    }

    /// <summary>
    /// Set a contact as the primary contact for an application
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <returns>Success response</returns>
    [HttpPatch("{id:int}/set-primary")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetPrimaryContact(int id)
    {
        try
        {
            var userId = _identityService.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("User not authenticated"));
            }

            var contact = await _context.Contacts
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == Enum.CommonStatus.Active && c.Application.UserId == userId);

            if (contact == null)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            // Unmark all other primary contacts for this application
            var existingPrimaryContacts = await _context.Contacts
                .Where(c => c.ApplicationId == contact.ApplicationId && c.IsPrimaryContact && c.Id != id && c.Status == Enum.CommonStatus.Active)
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

            return Ok(ApiResponse<object>.Success(new object(), "Contact set as primary successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting contact {ContactId} as primary", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while setting the contact as primary"));
        }
    }
}
