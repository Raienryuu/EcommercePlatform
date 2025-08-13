using Microsoft.AspNetCore.Mvc;

namespace ImageService;

public class MongoExceptionHandler(ILogger<MongoExceptionHandler> logger)
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
      Detail = "We could not connect to database. Try again later.",
      Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
    };
    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

    logger.LogCritical(
      "There was an error while trying to interact with database. Exception: {exception}",
      exception
    );

    return true;
  }
}
