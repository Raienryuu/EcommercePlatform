namespace Common;

public class UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger)
  : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
  public ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken
  )
  {
    httpContext.Response.StatusCode = 500;
    httpContext.Response.WriteAsync("Internal server error", cancellationToken: cancellationToken);
    logger.LogCritical("Unhandled exception occured: {message}", exception.Message);
    return ValueTask.FromResult(true);
  }
}
