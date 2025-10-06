using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace JobApplicationTrackerApi.SignalR;

/// <summary>
/// Provides a user identifier for SignalR connections based on the user's Name claim.
/// This allows SignalR to correlate connections with specific users.
/// </summary>
public class NameUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Gets the user identifier for the specified connection.
    /// </summary>
    /// <param name="connection">The connection to get the user identifier for.</param>
    /// <returns>The user identifier, which is the value of the Name claim, or null if the claim is not present.</returns>
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst(ClaimTypes.Name)?.Value;
    }
}