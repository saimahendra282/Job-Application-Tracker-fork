using System.ComponentModel.DataAnnotations;

namespace JobApplicationTrackerApi.DTO.Contacts;

/// <summary>
/// DTO for creating a new contact
/// </summary>
public sealed record CreateContactDto
{
    /// <summary>
    /// Foreign key to Application
    /// </summary>
    [Required]
    public int ApplicationId { get; init; }

    /// <summary>
    /// Name of the contact person
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Job title/position of the contact
    /// </summary>
    [StringLength(200)]
    public string? Position { get; init; }

    /// <summary>
    /// Email address of the contact
    /// </summary>
    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; init; }

    /// <summary>
    /// Phone number of the contact
    /// </summary>
    [Phone]
    [StringLength(50)]
    public string? Phone { get; init; }

    /// <summary>
    /// LinkedIn profile URL
    /// </summary>
    [Url]
    [StringLength(500)]
    public string? LinkedIn { get; init; }

    /// <summary>
    /// Additional notes about the contact
    /// </summary>
    [StringLength(2000)]
    public string? Notes { get; init; }

    /// <summary>
    /// Whether this is the primary contact for the application
    /// </summary>
    public bool IsPrimaryContact { get; init; } = false;
}
