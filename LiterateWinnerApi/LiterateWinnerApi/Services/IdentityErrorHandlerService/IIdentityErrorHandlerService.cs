using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JobApplicationTrackerApi.Services.IdentityErrorHandlerService;

/// <summary>
/// Interface for handling identity errors and adding them to the model state.
/// </summary>
public interface IIdentityErrorHandlerService
{
    /// <summary>
    /// Copies the errors from an <see cref="IdentityResult"/> into the supplied <see cref="ModelStateDictionary"/>.
    /// </summary>
    void AddErrors(ModelStateDictionary modelState, IdentityResult result);
}