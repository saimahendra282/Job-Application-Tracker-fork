using System.Security.Claims;
using FluentAssertions;
using JobApplicationTrackerApi.Services.IdentityService;
using Microsoft.AspNetCore.Http;
using Moq;

namespace JobApplicationTrackerApi.Tests.UnitTests.Services;

/// <summary>
/// Unit tests for IdentityService to verify user identity extraction from HTTP context.
/// Tests cover successful extraction, null handling, and error scenarios.
/// </summary>
public class IdentityServiceTests
{
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
    private readonly IdentityService _identityService;

    public IdentityServiceTests()
    {
        _contextAccessorMock = new Mock<IHttpContextAccessor>();
        _identityService = new IdentityService(_contextAccessorMock.Object);
    }

    [Fact]
    public void GetUserIdentity_WithValidClaim_ShouldReturnUserId()
    {
        // Arrange - Setup HTTP context with user claims
        var userId = "user-12345";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should return the correct user ID
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdentity_WithMultipleClaims_ShouldReturnNameClaim()
    {
        // Arrange - Setup HTTP context with multiple claims
        var userId = "admin-98765";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "admin@test.com"),
            new Claim(ClaimTypes.Name, userId),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("CustomClaim", "CustomValue")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should return the Name claim value
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdentity_WithoutNameClaim_ShouldReturnNull()
    {
        // Arrange - Setup HTTP context without Name claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "user@test.com"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should return null when Name claim is missing
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserIdentity_WithNullHttpContext_ShouldReturnNull()
    {
        // Arrange - Setup null HTTP context
        _contextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should gracefully return null
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserIdentity_WithNullUser_ShouldReturnNull()
    {
        // Arrange - Setup HTTP context with null user
        var httpContext = new DefaultHttpContext
        {
            User = null!
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should gracefully return null
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserIdentity_WithEmptyClaimsIdentity_ShouldReturnNull()
    {
        // Arrange - Setup HTTP context with empty claims
        var identity = new ClaimsIdentity(); // No authentication type
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should return null when no claims exist
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserIdentity_WithSpecialCharactersInUserId_ShouldReturnCorrectValue()
    {
        // Arrange - Setup HTTP context with special characters in user ID
        var userId = "user-123-abc@domain.com";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _identityService.GetUserIdentity();

        // Assert - Should handle special characters correctly
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdentity_CalledMultipleTimes_ShouldReturnConsistentResults()
    {
        // Arrange - Setup HTTP context
        var userId = "consistent-user-id";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act - Call method multiple times
        var result1 = _identityService.GetUserIdentity();
        var result2 = _identityService.GetUserIdentity();
        var result3 = _identityService.GetUserIdentity();

        // Assert - All results should be consistent
        result1.Should().Be(userId);
        result2.Should().Be(userId);
        result3.Should().Be(userId);
    }
}
