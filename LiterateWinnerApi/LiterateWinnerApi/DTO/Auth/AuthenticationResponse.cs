namespace JobApplicationTrackerApi.DTO.Auth;

public sealed record AuthenticationResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public Guid UserId { get; init; } = Guid.NewGuid();
    public string UserRole { get; init; } = string.Empty;
}