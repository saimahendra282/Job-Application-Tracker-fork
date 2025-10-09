using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using JobApplicationTrackerApi.Infrastructure;
using JobApplicationTrackerApi.Infrastructure.Options;
using JobApplicationTrackerApi.Services.TokenService;
using JobApplicationTrackerApi.Services.TokenService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace JobApplicationTrackerApi.Tests.UnitTests.Services;

/// <summary>
/// Unit tests for TokenService to verify JWT token generation, refresh tokens, and validation logic.
/// These tests ensure the security and functionality of the authentication token system.
/// </summary>
public class TokenServiceTests
{
    private readonly Mock<ILogger<TokenService>> _loggerMock;
    private readonly Mock<IOptions<JwtOptions>> _jwtOptionsMock;
    private readonly JwtOptions _jwtOptions;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        // Arrange - Setup common test dependencies
        _loggerMock = new Mock<ILogger<TokenService>>();
        _jwtOptions = new JwtOptions
        {
            SecurityKey = "this-is-a-very-secure-key-for-testing-purposes-with-enough-length",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenLifetime = TimeSpan.FromMinutes(15)
        };
        _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        _jwtOptionsMock.Setup(x => x.Value).Returns(_jwtOptions);

        _tokenService = new TokenService(_jwtOptionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_WithBasicUser_ShouldGenerateValidToken()
    {
        // Arrange - Create a basic user with minimal claims
        var user = new JwtUser(
            Id: "user-123",
            Roles: new List<string> { "User" }
        );

        // Act - Generate the access token
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - Verify token is generated and valid
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Audience);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "user-123");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateAccessToken_WithAdminUser_ShouldGenerateTokenWithNoExpiration()
    {
        // Arrange - Create an admin user (should get a token that never expires)
        var user = new JwtUser(
            Id: "admin-456",
            Roles: new List<string> { Roles.Admin }
        );

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - Admin tokens should have far-future expiration (DateTime.MaxValue)
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.MaxValue, TimeSpan.FromDays(1));
    }

    [Fact]
    public void GenerateAccessToken_WithCustomClaims_ShouldIncludeCustomClaims()
    {
        // Arrange - Create a user with custom claims
        var user = new JwtUser(
            Id: "user-789",
            Roles: new List<string> { "User" },
            CustomClaims: new Dictionary<string, string>
            {
                { "department", "Engineering" },
                { "level", "Senior" }
            }
        );

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - Verify custom claims are included in the token
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == "department" && c.Value == "Engineering");
        jwtToken.Claims.Should().Contain(c => c.Type == "level" && c.Value == "Senior");
    }

    [Fact]
    public void GenerateAccessToken_WithMultipleRoles_ShouldIncludeAllRoles()
    {
        // Arrange - Create a user with multiple roles
        var user = new JwtUser(
            Id: "user-multi",
            Roles: new List<string> { "User", "Manager", "Developer" }
        );

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - All roles should be present in the token
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(3);
        roleClaims.Should().Contain(c => c.Value == "User");
        roleClaims.Should().Contain(c => c.Value == "Manager");
        roleClaims.Should().Contain(c => c.Value == "Developer");
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateUniqueTokens()
    {
        // Act - Generate multiple refresh tokens
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();
        var token3 = _tokenService.GenerateRefreshToken();

        // Assert - Each refresh token should be unique and non-empty
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token3.Should().NotBeNullOrEmpty();
        
        token1.Should().NotBe(token2);
        token2.Should().NotBe(token3);
        token1.Should().NotBe(token3);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateBase64String()
    {
        // Act
        var token = _tokenService.GenerateRefreshToken();

        // Assert - Refresh token should be a valid Base64 string
        token.Should().NotBeNullOrEmpty();
        var bytes = Convert.FromBase64String(token);
        bytes.Should().HaveCount(32); // 32 bytes of random data
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithValidToken_ShouldReturnPrincipal()
    {
        // Arrange - Create a token that will be expired
        var user = new JwtUser(
            Id: "user-expired",
            Roles: new List<string> { "User" }
        );
        var token = _tokenService.GenerateAccessToken(user);

        // Act - Extract principal from the token (even if expired)
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);

        // Assert - Principal should be valid with correct claims
        principal.Should().NotBeNull();
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be("user-expired");
        principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be("User");
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ShouldThrowException()
    {
        // Arrange - Create an invalid token string
        var invalidToken = "this-is-not-a-valid-jwt-token";

        // Act & Assert - Should throw an exception for invalid token
        var act = () => _tokenService.GetPrincipalFromExpiredToken(invalidToken);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithTokenSignedByDifferentKey_ShouldThrowException()
    {
        // Arrange - Create a token with a different security key
        var differentOptions = new JwtOptions
        {
            SecurityKey = "completely-different-security-key-that-should-fail-validation",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenLifetime = TimeSpan.FromMinutes(15)
        };
        var differentOptionsMock = new Mock<IOptions<JwtOptions>>();
        differentOptionsMock.Setup(x => x.Value).Returns(differentOptions);
        var differentTokenService = new TokenService(differentOptionsMock.Object, _loggerMock.Object);

        var user = new JwtUser("user-123", new List<string> { "User" });
        var tokenWithDifferentKey = differentTokenService.GenerateAccessToken(user);

        // Act & Assert - Should throw SecurityTokenException due to key mismatch
        var act = () => _tokenService.GetPrincipalFromExpiredToken(tokenWithDifferentKey);
        act.Should().Throw<SecurityTokenException>();
    }

    [Fact]
    public void GenerateAccessToken_WithNullRoles_ShouldGenerateValidToken()
    {
        // Arrange - User with no roles
        var user = new JwtUser(
            Id: "user-no-roles",
            Roles: null
        );

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - Token should still be valid even without roles
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "user-no-roles");
    }

    [Fact]
    public void GenerateAccessToken_WithEmptyCustomClaims_ShouldGenerateValidToken()
    {
        // Arrange - User with empty custom claims dictionary
        var user = new JwtUser(
            Id: "user-empty-claims",
            Roles: new List<string> { "User" },
            CustomClaims: new Dictionary<string, string>()
        );

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert - Token should be valid
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name);
    }
}
