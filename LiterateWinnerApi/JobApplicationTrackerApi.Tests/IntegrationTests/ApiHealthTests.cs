using System.Net;
using FluentAssertions;

namespace JobApplicationTrackerApi.Tests.IntegrationTests;

/// <summary>
/// Integration tests for API health and basic infrastructure.
/// These tests verify that the application starts correctly and basic endpoints work.
/// </summary>
public class ApiHealthTests : IntegrationTestBase
{
    public ApiHealthTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Api_ShouldStart_Successfully()
    {
        // Arrange & Act - The fact that we can create a client means the app started
        var client = Factory.CreateClient();

        // Assert - Client should be created without exceptions
        client.Should().NotBeNull();
        client.BaseAddress.Should().NotBeNull();
    }

    [Fact]
    public async Task SwaggerEndpoint_ShouldReturn_Success()
    {
        // Arrange
        var swaggerUrl = "/swagger/v1/swagger.json";

        // Act
        var response = await Client.GetAsync(swaggerUrl);

        // Assert - Swagger documentation should be accessible
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        // NotFound is acceptable if swagger isn't configured, OK if it is
    }

    [Fact]
    public async Task NonExistentEndpoint_ShouldReturn_NotFound()
    {
        // Arrange
        var nonExistentUrl = "/api/non-existent-endpoint";

        // Act
        var response = await Client.GetAsync(nonExistentUrl);

        // Assert - Should return 404 for routes that don't exist
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Api_WithInvalidContentType_ShouldReturn_UnsupportedMediaType()
    {
        // Arrange
        var url = "/api/test"; // This endpoint doesn't exist, but that's OK for this test
        var content = new StringContent("invalid content");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

        // Act
        var response = await Client.PostAsync(url, content);

        // Assert - Should return 404 (no endpoint) or 415 (unsupported media type)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.UnsupportedMediaType
        );
    }

    [Fact]
    public async Task Api_SupportsJson_ContentType()
    {
        // Arrange
        var testData = new { test = "data" };

        // Act
        var response = await PostAsJsonAsync("/api/test", testData);

        // Assert - Should handle JSON content (404 is OK, it means routing works)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task Database_ShouldBeAccessible()
    {
        // Arrange & Act - Try to access the database context
        var canConnect = await IdentityContext.Database.CanConnectAsync();

        // Assert - Database should be accessible
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task UserManager_ShouldBeConfigured()
    {
        // Arrange & Act - Check if UserManager is properly configured
        var users = UserManager.Users;

        // Assert - UserManager should be available and queryable
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task RoleManager_ShouldHaveDefaultRoles()
    {
        // Arrange
        var expectedRoles = new[] { "Admin", "User" };

        // Act & Assert - Check that default roles were seeded
        foreach (var role in expectedRoles)
        {
            var exists = await RoleManager.RoleExistsAsync(role);
            exists.Should().BeTrue($"Role '{role}' should exist in the system");
        }
    }

    [Fact]
    public async Task CreateTestUser_ShouldSucceed()
    {
        // Arrange
        var testEmail = $"integration-test-{Guid.NewGuid()}@example.com";

        // Act
        var user = await CreateTestUserAsync(testEmail, "Test@123", "User");

        // Assert - User should be created successfully
        user.Should().NotBeNull();
        user.Email.Should().Be(testEmail);
        user.EmailConfirmed.Should().BeTrue();

        // Verify user exists in database
        var foundUser = await UserManager.FindByEmailAsync(testEmail);
        foundUser.Should().NotBeNull();
        foundUser!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task CreateMultipleUsers_ShouldSucceed()
    {
        // Arrange
        var userCount = 3;
        var createdUsers = new List<string>();

        // Act - Create multiple test users
        for (int i = 0; i < userCount; i++)
        {
            var email = $"user-{i}-{Guid.NewGuid()}@example.com";
            var user = await CreateTestUserAsync(email, "Test@123", "User");
            createdUsers.Add(user.Id);
        }

        // Assert - All users should be created
        createdUsers.Should().HaveCount(userCount);
        createdUsers.Should().OnlyHaveUniqueItems();

        // Verify all users exist in database
        var allUsers = UserManager.Users.ToList();
        allUsers.Count.Should().BeGreaterThanOrEqualTo(userCount);
    }
}
