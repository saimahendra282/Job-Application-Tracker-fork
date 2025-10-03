namespace JobApplicationTrackerApi.Infrastructure.Middleware;

public class RequestContextLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestContextLoggingMiddleware> logger)
{
    public Task Invoke(HttpContext context)
    {
        logger.LogInformation($"Request ID :{context.TraceIdentifier}:{context.Request.Method}:" +
                              context.Request.Path);
        return next.Invoke(context);
    }
}