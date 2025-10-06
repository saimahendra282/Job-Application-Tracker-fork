using JobApplicationTrackerApi.DTO.Contacts;

namespace JobApplicationTrackerApi.Services.ContactsService;

/// <summary>
/// Service interface for managing application contacts
/// </summary>
public interface IContactsService
{
    /// <summary>
    /// Get all contacts for a specific application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>List of contacts for the application</returns>
    Task<List<ContactResponseDto>> GetContactsByApplicationAsync(int applicationId, string userId);

    /// <summary>
    /// Get a specific contact by ID
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The contact details</returns>
    Task<ContactResponseDto?> GetContactByIdAsync(int id, string userId);

    /// <summary>
    /// Create a new contact
    /// </summary>
    /// <param name="createContactDto">The contact data</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The created contact</returns>
    Task<ContactResponseDto> CreateContactAsync(CreateContactDto createContactDto, string userId);

    /// <summary>
    /// Update an existing contact
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="updateContactDto">The updated contact data</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The updated contact</returns>
    Task<ContactResponseDto> UpdateContactAsync(int id, UpdateContactDto updateContactDto, string userId);

    /// <summary>
    /// Delete a contact
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteContactAsync(int id, string userId);

    /// <summary>
    /// Set a contact as the primary contact for an application
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>True if set successfully</returns>
    Task<bool> SetPrimaryContactAsync(int id, string userId);

    /// <summary>
    /// Get the primary contact for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The primary contact or null if none exists</returns>
    Task<ContactResponseDto?> GetPrimaryContactAsync(int applicationId, string userId);

    /// <summary>
    /// Search contacts by name or email
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="searchTerm">The search term</param>
    /// <param name="userId">The user ID</param>
    /// <returns>List of matching contacts</returns>
    Task<List<ContactResponseDto>> SearchContactsAsync(int applicationId, string searchTerm, string userId);
}
