namespace JobApplicationTrackerApi.Services.IdentityService;

/// <summary>
/// Provides methods to retrieve the identity of the current user.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Gets the identity of the current user from the HTTP context.
    /// </summary>
    /// <returns>The user identifier as a string, or null if no user is authenticated.</returns>
    string? GetUserIdentity();
}