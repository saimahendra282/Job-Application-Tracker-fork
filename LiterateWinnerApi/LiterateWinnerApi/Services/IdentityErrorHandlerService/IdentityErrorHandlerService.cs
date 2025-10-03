using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JobApplicationTrackerApi.Services.IdentityErrorHandlerService;

public sealed class IdentityErrorHandlerService : IIdentityErrorHandlerService
{
    public void AddErrors(ModelStateDictionary modelState, IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            modelState.TryAddModelError(error.Code, error.Description);
        }
    }
}