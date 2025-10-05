using JobApplicationTrackerApi.DTO.Contacts;
using JobApplicationTrackerApi.Services.ContactsService;
using JobApplicationTrackerApi.Services.IdentityService;
using JobApplicationTrackerApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTrackerApi.Controllers;

/// <summary>
/// Controller for managing application contacts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController(
    IContactsService contactsService,
    IIdentityService identityService,
    ILogger<ContactsController> logger
) : ControllerBase
{
    private readonly IContactsService _contactsService = contactsService ?? throw new ArgumentNullException(nameof(contactsService));
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

            var contacts = await _contactsService.GetContactsByApplicationAsync(applicationId, userId);
            return Ok(ApiResponse<List<ContactResponseDto>>.Success(contacts, "Contacts retrieved successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Application not found or access denied for application {ApplicationId}", applicationId);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
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

            var contact = await _contactsService.GetContactByIdAsync(id, userId);
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

            var contact = await _contactsService.CreateContactAsync(createContactDto, userId);
            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, 
                ApiResponse<ContactResponseDto>.Success(contact, "Contact created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Application not found or access denied for application {ApplicationId}", createContactDto.ApplicationId);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
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

            var contact = await _contactsService.UpdateContactAsync(id, updateContactDto, userId);
            return Ok(ApiResponse<ContactResponseDto>.Success(contact, "Contact updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact not found or access denied for contact {ContactId}", id);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
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

            var result = await _contactsService.DeleteContactAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            return Ok(ApiResponse<object>.Success(new object(), "Contact deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact not found or access denied for contact {ContactId}", id);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
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

            var result = await _contactsService.SetPrimaryContactAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Failure("Contact not found"));
            }

            return Ok(ApiResponse<object>.Success(new object(), "Contact set as primary successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact not found or access denied for contact {ContactId}", id);
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting contact {ContactId} as primary", id);
            return StatusCode(500, ApiResponse<object>.Failure("An error occurred while setting the contact as primary"));
        }
    }
}
