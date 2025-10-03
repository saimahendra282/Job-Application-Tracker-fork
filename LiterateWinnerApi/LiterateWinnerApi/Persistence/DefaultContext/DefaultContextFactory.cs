using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobApplicationTrackerApi.Persistence.DefaultContext;

public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DefaultContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Helper method to run migrations programmatically
    /// </summary>
    public static void RunMigrations()
    {
        var factory = new DefaultContextFactory();
        using var context = factory.CreateDbContext([]);

        Console.WriteLine("Applying migrations for DefaultContext...");

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