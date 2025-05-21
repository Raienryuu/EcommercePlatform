using OrderService.Models;

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
    EventId = 1005,
    Level = LogLevel.Warning,
    Message = "Unable to save changes for order: {OrderId}"
  )]
  public static partial void OrderChangesSaveFailure(this ILogger logger, Guid orderId);

  [LoggerMessage(
    EventId = 2002,
    Level = LogLevel.Information,
    Message = "Started order cancellation for order Id: {OrderId}"
  )]
  public static partial void StartedOrderCancellation(this ILogger logger, Guid orderId);

  [LoggerMessage(
    EventId = 2003,
    Level = LogLevel.Warning,
    Message = "Order with id : {OrderId}, could not be cancelled, current status was {Status}"
  )]
  public static partial void UnableToCancelOrder(this ILogger logger, Guid orderId, OrderStatus.Type status);

  [LoggerMessage(
    EventId = 2004,
    Level = LogLevel.Error,
    Message = "Tried to refund order that does not exists, order with id : {OrderId}"
  )]
  public static partial void RefundingNonexistentOrder(this ILogger logger, Guid orderId);

  [LoggerMessage(
    EventId = 2005,
    Level = LogLevel.Information,
    Message = "Order refunded, order with id : {OrderId}, amount: {Amount}, currency: {CurrencyISO}"
  )]
  public static partial void OrderRefunded(this ILogger logger, Guid orderId, int amount, string currencyISO);

  [LoggerMessage(
    EventId = 2006,
    Level = LogLevel.Error,
    Message = "Refunding without paymentId for  order with id : {OrderId}"
  )]
  public static partial void RefundingWithoutPaymentId(this ILogger logger, Guid orderId);

  [LoggerMessage(
    EventId = 2007,
    Level = LogLevel.Warning,
    Message = "Unable to cancel payment intent for order with id : {OrderId}, reason: {Message}"
  )]
  public static partial void UnableToCancelPaymentIntent(this ILogger logger, Guid orderId, string Message);

  [LoggerMessage(
    EventId = 2008,
    Level = LogLevel.Error,
    Message = "Order with id : {OrderId}, cancellation meet an error: {Message}"
  )]
  public static partial void UnableToCancelOrder(this ILogger logger, Guid orderId, string message);

  [LoggerMessage(
    EventId = 2009,
    Level = LogLevel.Information,
    Message = "Cancelled payment intent for order with id : {OrderId}"
  )]
  public static partial void CancelledPaymentIntent(this ILogger logger, Guid orderId);
}
