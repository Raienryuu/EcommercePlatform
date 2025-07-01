using Microsoft.AspNetCore.Mvc;

namespace Common;

public class UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger)
  : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken
  )
  {
    httpContext.Response.StatusCode = 500;

    var problemDetails = new ProblemDetails
    {
      Status = 500,
      Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
      Title = "Internal server error",
      Detail = "Unexpected error happened on our side.",
      Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
    };
    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
    logger.LogCritical("Unhandled exception occured: {message}", exception.Message);
    return true;
  }
}
