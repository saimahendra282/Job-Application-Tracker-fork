using JobApplicationTrackerApi.Persistence.DefaultContext;
using JobApplicationTrackerApi.Persistence.IdentityContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Startup;

public static class Migration
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        // Skip migrations if running in Kubernetes
        if (app.IsInKubernetes())
        {
            return;
        }

        // Apply migrations for Identity context
        app.MigrateDbContext<IdentityContext>((_, services) =>
        {
            var logger = services.GetRequiredService<ILogger<IdentityContext>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityContextSeed.SeedAsync(logger, roleManager).Wait();
        });

        // Apply migrations for Default context
        app.MigrateDbContext<DefaultContext>((context, services) => { });
    }

    private static bool IsInKubernetes(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var kubernetesServiceHost = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
        return !string.IsNullOrEmpty(kubernetesServiceHost);
    }

    private static void MigrateDbContext<TContext>(this IApplicationBuilder app,
        Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);

            // Apply migrations if any pending
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
                logger.LogInformation("Applied migrations for context {DbContextName}", typeof(TContext).Name);
            }

            // Seed data
            InvokeSeeder(seeder, context, services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name);
            throw;
        }
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services)
        where TContext : DbContext
    {
        seeder(context, services);
        context.SaveChanges();
    }
}