using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTrackerApi.Extensions;

public static class ProblemDetailsExtensions
{
    public static ObjectResult ToObjectResult(this ProblemDetails problemDetails)
    {
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}