using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobApplicationTrackerApi.Persistence.IdentityContext;

public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new IdentityContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Helper method to run migrations programmatically
    /// </summary>
    public static void RunMigrations()
    {
        var factory = new IdentityContextFactory();
        using var context = factory.CreateDbContext([]);

        Console.WriteLine("Applying migrations for IdentityContext...");

        // Apply any pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully!");
        }
        else
        {
            Console.WriteLine("No pending migrations found.");
        }
    }
}