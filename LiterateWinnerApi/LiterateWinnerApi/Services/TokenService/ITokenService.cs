using System.Security.Claims;
using JobApplicationTrackerApi.Services.TokenService.Models;

namespace JobApplicationTrackerApi.Services.TokenService;

/// <summary>
/// Interface for token generation and validation services used for JWT authentication.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a new JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The user data to include in the token claims.</param>
    /// <returns>A JWT access token string.</returns>
    string GenerateAccessToken(JwtUser user);

    /// <summary>
    /// Generates a new refresh token for authentication renewal.
    /// </summary>
    /// <returns>A cryptographically secure refresh token string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts and validates the claims principal from an expired JWT token.
    /// </summary>
    /// <param name="token">The expired JWT token.</param>
    /// <returns>A ClaimsPrincipal object containing the token's claims.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}