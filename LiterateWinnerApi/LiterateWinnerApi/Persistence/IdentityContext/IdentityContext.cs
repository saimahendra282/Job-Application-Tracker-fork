using JobApplicationTrackerApi.Persistence.IdentityContext.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Persistence.IdentityContext;

/// <summary>
/// Identity Context
/// </summary>
/// <param name="options">DbContextOptions</param>
public class IdentityContext(
    DbContextOptions<IdentityContext> options
)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().HasIndex(x => x.RefreshToken).IsUnique();

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
    }
}