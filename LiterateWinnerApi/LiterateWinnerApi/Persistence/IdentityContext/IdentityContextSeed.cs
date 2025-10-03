using System.Reflection;
using JobApplicationTrackerApi.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace JobApplicationTrackerApi.Persistence.IdentityContext;

public static class IdentityContextSeed
{
    public static async Task SeedAsync(
        ILogger<IdentityContext> logger,
        RoleManager<IdentityRole> roleManager)
    {
        try
        {
            logger.LogInformation("Starting identity initialization...");

            var roles = typeof(Roles).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var role in roles)
            {
                var roleName = role?.GetValue(null)?.ToString() ?? "";

                if (await roleManager.RoleExistsAsync(roleName)) continue;

                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                VerifyIdentityResults(result);
            }

            logger.LogInformation("Identity initialization finished successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occured during identity initialization");
            throw;
        }
    }

    private static void VerifyIdentityResults(params IdentityResult[] results)
    {
        foreach (var result in results)
        {
            if (result.Succeeded)
                continue;

            var errorMsg = string.Join("Error details: ", result.Errors);
            throw new InvalidOperationException(errorMsg);
        }
    }
}