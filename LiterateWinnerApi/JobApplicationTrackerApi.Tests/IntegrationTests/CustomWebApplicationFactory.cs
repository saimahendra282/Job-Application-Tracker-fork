using JobApplicationTrackerApi.Persistence.IdentityContext;
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
        builder.ConfigureServices(services =>
        {
            // Remove ONLY the IdentityContext DbContext registration, preserving Identity services
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IdentityContext));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove DbContextOptions<IdentityContext>
            var dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<IdentityContext>));
            if (dbContextOptionsDescriptor != null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            // Remove any Redis or distributed cache services
            services.RemoveAll(typeof(StackExchange.Redis.IConnectionMultiplexer));

            // Add in-memory distributed cache instead of Redis
            services.AddDistributedMemoryCache();
        });

        builder.ConfigureTestServices(services =>
        {
            // Add Identity DbContext using in-memory database
            // Use a shared root to maintain data between contexts in the same test
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseInMemoryDatabase("TestIdentityDb", _databaseRoot);
            });

            // Ensure the database is created and seeded
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            identityContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
