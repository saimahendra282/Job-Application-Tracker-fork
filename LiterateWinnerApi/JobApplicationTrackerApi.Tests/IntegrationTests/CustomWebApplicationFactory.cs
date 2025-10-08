using JobApplicationTrackerApi.Persistence.IdentityContext;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobApplicationTrackerApi.Tests.IntegrationTests;

/// <summary>
/// Custom Web Application Factory for integration testing.
/// Creates an in-memory test server with isolated database for each test.
/// This allows us to test the full HTTP pipeline including middleware, routing, and controllers.
/// </summary>
/// <typeparam name="TProgram">The entry point of the application being tested</typeparam>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set the environment to Testing first so the main app skips SQL Server registration
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove any Redis or distributed cache services
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();

            // Add in-memory distributed cache instead of Redis
            services.AddDistributedMemoryCache();
        });

        builder.ConfigureTestServices(services =>
        {
            // Since the main app skips database registration in Testing environment,
            // we can safely add our in-memory databases here without conflicts

            // Add IdentityContext
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseInMemoryDatabase("TestIdentityDb", _databaseRoot);
                // Disable service provider caching to avoid conflicts
                options.EnableServiceProviderCaching(false);
                // Enable sensitive data logging for better test debugging
                options.EnableSensitiveDataLogging();
            });

            // Add DefaultContext
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("TestDefaultDb", _databaseRoot);
                // Disable service provider caching to avoid conflicts
                options.EnableServiceProviderCaching(false);
                // Enable sensitive data logging for better test debugging
                options.EnableSensitiveDataLogging();
            });

            // Ensure the databases are created and seeded
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            identityContext.Database.EnsureCreated();

            var defaultContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            defaultContext.Database.EnsureCreated();
        });
    }
}
