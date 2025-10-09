using JobApplicationTrackerApi.Services.TokenService;
using JobApplicationTrackerApi.Services.TokenService.Models;
using JobApplicationTrackerApi.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace JobApplicationTrackerApi.Tests.SecurityTests;

public class SecurityTests
{
    private readonly TokenService _tokenService;
    private readonly Mock<IOptions<JwtOptions>> _jwtOptionsMock;
    private readonly Mock<ILogger<TokenService>> _loggerMock;

    public SecurityTests()
    {
        _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtOptions
        {
            SecurityKey = "supersecretkeythatislongenoughforjwttokens",
            Issuer = "testissuer",
            Audience = "testaudience",
            AccessTokenLifetime = TimeSpan.FromMinutes(15),
            RefreshTokenLifetime = TimeSpan.FromDays(7)
        });

        _loggerMock = new Mock<ILogger<TokenService>>();
        _tokenService = new TokenService(_jwtOptionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_ReturnsValidJwt()
    {
        // Arrange
        var user = new JwtUser("user123", new[] { "User" });

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify token can be read
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal("testissuer", jwtToken.Issuer);
        Assert.Equal("testaudience", jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" && c.Value == user.Id);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User");
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsSecureToken()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        Assert.True(refreshToken.Length >= 32); // Should be cryptographically secure
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ThrowsException()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _tokenService.GetPrincipalFromExpiredToken(invalidToken));
    }
}