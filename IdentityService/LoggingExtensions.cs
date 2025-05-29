namespace IdentityService;

public static partial class LoggingExtensions
{
  [LoggerMessage(1000, LogLevel.Information, "Registered new user: {OrderId}")]
  public static partial void RegisteredNewUser(this ILogger logger, Guid orderId);

  [LoggerMessage(1001, LogLevel.Information, "User {UserId} used invalid password")]
  public static partial void InvalidLogin(this ILogger logger, Guid userId);

  [LoggerMessage(1002, LogLevel.Information, "User {UserId} logged in successfully")]
  public static partial void SuccessfullLogin(this ILogger logger, Guid userId);
}
