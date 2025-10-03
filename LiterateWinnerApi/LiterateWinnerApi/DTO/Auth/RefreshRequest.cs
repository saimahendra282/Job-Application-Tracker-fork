namespace JobApplicationTrackerApi.DTO.Auth;

public sealed record RefreshRequest
{
    public required string AccessToken { get; init; } = string.Empty;
    public required string RefreshToken { get; init; } = string.Empty;
}