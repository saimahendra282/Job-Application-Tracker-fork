using System.Security.Claims;

namespace JobApplicationTrackerApi.Services.IdentityService;

public sealed class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public string? GetUserIdentity()
    {
        try
        {
            return context.HttpContext!.User.FindFirst(ClaimTypes.Name)!.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }
}