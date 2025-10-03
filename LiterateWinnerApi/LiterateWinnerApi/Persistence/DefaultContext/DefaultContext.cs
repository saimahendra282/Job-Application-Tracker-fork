using JobApplicationTrackerApi.Enum;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
using LiterateWinnerApi.Persistence.DefaultContext.Entity;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Persistence.DefaultContext;

/// <summary>
/// The default database context for the application.
/// </summary>
/// <param name="options">The options for configuring the context.</param>
public class DefaultContext(
    DbContextOptions<DefaultContext> options
)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            var baseEntity = (BaseEntity)entityEntry.Entity;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    baseEntity.CreatedUtc = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                {
                    baseEntity.UpdatedUtc = DateTime.UtcNow;

                    // Set DeletedDate only when Status is 99
                    if (baseEntity.Status == CommonStatus.Delete)
                    {
                        baseEntity.DeletedUtc = DateTime.UtcNow;
                    }

                    break;
                }
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Entities
    }
}