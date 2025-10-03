namespace JobApplicationTrackerApi.DTO.Auth;

public sealed record UserLoginDto
{
    public required string UserName { get; init; } = string.Empty;
    public required string Password { get; init; } = string.Empty;
}