namespace JobApplicationTrackerApi.Services.TokenService.Models;

/// <summary>
/// Represents a user for JWT token generation with identity information and claims.
/// </summary>
/// <param name="Id">The unique identifier for the user.</param>
/// <param name="Roles">The collection of roles assigned to the user.</param>
/// <param name="CustomClaims">Additional custom claims to include in the JWT token.</param>
public sealed record JwtUser(
    string Id,
    IEnumerable<string>? Roles,
    Dictionary<string, string>? CustomClaims = null
);