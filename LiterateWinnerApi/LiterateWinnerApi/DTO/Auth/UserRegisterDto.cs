namespace JobApplicationTrackerApi.DTO.Auth;

public sealed record UserRegisterDto
{
    public string UserName { get; set; } = string.Empty;
}