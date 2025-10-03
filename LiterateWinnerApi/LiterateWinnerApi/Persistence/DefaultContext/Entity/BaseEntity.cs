using JobApplicationTrackerApi.Enum;

namespace JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

public abstract class BaseEntity
{
    public DateTime CreatedUtc { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? UpdatedUtc { get; set; }

    public string? UpdatedBy { get; set; } = string.Empty;

    public DateTime? DeletedUtc { get; set; }

    public string? DeletedBy { get; set; } = string.Empty;

    public CommonStatus Status { get; set; }
}