using JobApplicationTrackerApi.Enum;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;
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
    /// <summary>
    /// Gets or sets the Applications DbSet
    /// </summary>
    public DbSet<Application> Applications { get; set; }

    /// <summary>
    /// Gets or sets the Interviews DbSet
    /// </summary>
    public DbSet<Interview> Interviews { get; set; }

    /// <summary>
    /// Gets or sets the Notes DbSet
    /// </summary>
    public DbSet<Note> Notes { get; set; }

    /// <summary>
    /// Gets or sets the Contacts DbSet
    /// </summary>
    public DbSet<Contact> Contacts { get; set; }

    /// <summary>
    /// Gets or sets the Documents DbSet
    /// </summary>
    public DbSet<Document> Documents { get; set; }

    /// <summary>
    /// Gets or sets the StatusHistory DbSet
    /// </summary>
    public DbSet<StatusHistory> StatusHistory { get; set; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
    /// <returns>A task that represents the asynchronous save operation</returns>
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

    /// <summary>
    /// Configures the model for the database context
    /// </summary>
    /// <param name="builder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        // Configure Application entity
        builder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Position).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.JobUrl).HasMaxLength(500);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SalaryMin).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SalaryMax).HasColumnType("decimal(18,2)");
            entity.Property(e => e.JobDescription).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Requirements).HasColumnType("nvarchar(max)");

            // Foreign key to ApplicationUser (no navigation property configured)
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);

            // Indexes for performance
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.ApplicationDate);
            entity.HasIndex(e => e.UserId);
        });

        // Configure Interview entity
        builder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InterviewerName).HasMaxLength(200);
            entity.Property(e => e.InterviewerPosition).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.MeetingLink).HasMaxLength(500);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Outcome).HasMaxLength(100);

            // Foreign key relationship
            entity.HasOne(e => e.Application)
                .WithMany(a => a.Interviews)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.InterviewDate);
            entity.HasIndex(e => e.ApplicationId);
        });

        // Configure Note entity
        builder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired().HasColumnType("nvarchar(max)");

            // Foreign key relationship
            entity.HasOne(e => e.Application)
                .WithMany(a => a.Notes)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.NoteType);
        });

        // Configure Contact entity
        builder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Position).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.LinkedIn).HasMaxLength(500);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            // Foreign key relationship
            entity.HasOne(e => e.Application)
                .WithMany(a => a.Contacts)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.Email);
        });

        // Configure Document entity
        builder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.MimeType).HasMaxLength(100);

            // Foreign key relationship
            entity.HasOne(e => e.Application)
                .WithMany(a => a.Documents)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.DocumentType);
        });

        // Configure StatusHistory entity
        builder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            // Foreign key relationship
            entity.HasOne(e => e.Application)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.ChangedAt);
        });
    }
}