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

  [LoggerMessage(
    EventId = 1004,
    Level = LogLevel.Information,
    Message = "A successful payment for {Amount} was made."
  )]
  public static partial void OrderSuccessfulPaymentInfo(this ILogger logger, long amount);

  [LoggerMessage(
    EventId = 2001,
    Level = LogLevel.Information,
    Message = "Sent ReserveOrderProductsCommand for order Id: {OrderId}"
  )]
  public static partial void OrderReserveOrderCommandSent(this ILogger logger, Guid orderId);

  [LoggerMessage(
    EventId = 2002,
    Level = LogLevel.Information,
    Message = "Started order cancellation for order Id: {OrderId}"
  )]
  public static partial void StartedOrderCancellation(this ILogger logger, Guid orderId);

  [LoggerMessage(
  EventId = 2003,
  Level = LogLevel.Warning,
  Message = "Order with id : {OrderId}, could not be cancelled"
)]
  public static partial void UnableToCancelOrder(this ILogger logger, Guid orderId);


}
