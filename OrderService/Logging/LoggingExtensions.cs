namespace OrderService.Logging;

public static partial class LoggerExtensions
{
  [LoggerMessage(EventId = 1001, Level = LogLevel.Warning, Message = "Order not found")]
  public static partial void OrderNotFound(this ILogger logger);

  [LoggerMessage(
    EventId = 1002,
    Level = LogLevel.Warning,
    Message = "Order currency does not match, expedted: {Expected}, got: {Given}"
  )]
  public static partial void OrderCurrencyMismatch(this ILogger logger, string Expected, string Given);

  [LoggerMessage(
    EventId = 1003,
    Level = LogLevel.Warning,
    Message = "Trying to set total cost of order again, orderId: {OrderId}, previousPrice: {PreviousPrice}"
  )]
  public static partial void OrderPriceIsReassigned(this ILogger logger, Guid orderId, int previousPrice);
}
