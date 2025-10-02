using System.ComponentModel.DataAnnotations;

namespace JobApplicationTrackerApi.Infrastructure.Options;

public sealed record JwtOptions
{
    public const string Key = "Jwt";

    [Required] public string Issuer { get; init; } = string.Empty;

    [Required] public string Audience { get; init; } = string.Empty;

    [Required] public string SecurityKey { get; init; } = string.Empty;

    [Range(typeof(TimeSpan), "0.00:00:01.0", "10675199.02:48:05.4775807")]
    public TimeSpan AccessTokenLifetime { get; init; }

    [Range(typeof(TimeSpan), "0.00:00:01.0", "10675199.02:48:05.4775807")]
    public TimeSpan RefreshTokenLifetime { get; init; }
}