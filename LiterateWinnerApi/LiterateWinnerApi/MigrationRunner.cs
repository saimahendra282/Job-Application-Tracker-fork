using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.IdentityContext;

namespace JobApplicationTrackerApi;

/// <summary>
/// Helper class to run migrations directly without starting the full application
/// </summary>
public static class MigrationRunner
{
    /// <summary>
    /// Run this with: dotnet run --project ChatBotApi.csproj -- migrate
    /// </summary>
    public static void RunMigrations(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("migrate", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Starting database migrations...");

            try
            {
                // Run migrations for IdentityContext
                IdentityContextFactory.RunMigrations();

                // Run migrations for DefaultContext
                DefaultContextFactory.RunMigrations();

                Console.WriteLine("All migrations completed successfully!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error running migrations: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
            }

            // Exit after running migrations
            Environment.Exit(0);
        }
    }
}