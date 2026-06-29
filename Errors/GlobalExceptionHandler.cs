using Microsoft.AspNetCore.Diagnostics;

namespace AI_genda_API.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger ): IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _Logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
       
        _Logger.LogError("An unhandled exception occurred: {message}",exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails);

        return true;
    }
}
