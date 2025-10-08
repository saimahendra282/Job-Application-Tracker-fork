using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using JobApplicationTrackerApi.Persistence.IdentityContext;
using JobApplicationTrackerApi.Persistence.IdentityContext.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTrackerApi.Tests.IntegrationTests;

/// <summary>
/// Base class for integration tests providing common setup and helper methods.
/// Manages test client creation, authentication, and database seeding.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    protected readonly CustomWebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly IdentityContext IdentityContext;
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly RoleManager<IdentityRole> RoleManager;

    protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();

        // Create a scope for accessing services
        Scope = factory.Services.CreateScope();
        IdentityContext = Scope.ServiceProvider.GetRequiredService<IdentityContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed default roles
        SeedRolesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Seeds default roles (Admin, User) into the test database
    /// </summary>
    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await RoleManager.RoleExistsAsync(role))
            {
                await RoleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    /// <summary>
    /// Creates a test user with specified role
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="role">User role (default: "User")</param>
    /// <returns>The created ApplicationUser</returns>
    protected async Task<ApplicationUser> CreateTestUserAsync(
        string email = "test@example.com",
        string password = "Test@123",
        string role = "User")
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = "Test User",
            CreateUtc = DateTime.UtcNow
        };

        var result = await UserManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await UserManager.AddToRoleAsync(user, role);
        return user;
    }

    /// <summary>
    /// Authenticates a test user and sets the Authorization header
    /// Note: This is a placeholder - implement actual authentication once auth endpoints exist
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    protected async Task<string> AuthenticateUserAsync(string email, string password)
    {
        // TODO: Implement actual authentication call when auth endpoints are available
        // For now, return a mock token
        var mockToken = "mock-jwt-token-for-testing";
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mockToken);
        return mockToken;
    }

    /// <summary>
    /// Clears authentication headers
    /// </summary>
    protected void ClearAuthentication()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Helper method to POST JSON data
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await Client.PostAsync(url, content);
    }

    /// <summary>
    /// Helper method to PUT JSON data
    /// </summary>
    protected async Task<HttpResponseMessage> PutAsJsonAsync<T>(string url, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await Client.PutAsync(url, content);
    }

    /// <summary>
    /// Helper method to deserialize response content
    /// </summary>
    protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public void Dispose()
    {
        Client?.Dispose();
        Scope?.Dispose();
        IdentityContext?.Dispose();
        GC.SuppressFinalize(this);
    }
}
