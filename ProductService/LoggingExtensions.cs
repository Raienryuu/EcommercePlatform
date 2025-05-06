namespace ProductService;

public static partial class LoggingExtenstions
{
  [LoggerMessage(1000, LogLevel.Error, "Couldn't find needed products for order: {OrderId}")]
  public static partial void CouldntFindProducts(this ILogger logger, Guid orderId);

  [LoggerMessage(1001, LogLevel.Error, "Couldn't find delivery item with id: {DeliveryId}")]
  public static partial void CouldntFindDeliveryItem(this ILogger logger, Guid deliveryId);

  [LoggerMessage(
    EventId = 1002,
    Level = LogLevel.Information,
    Message = "Calculated price for order: {OrderId}, price: {TotalPrice}"
  )]
  public static partial void OrderTotalCalculatedSuccessfully(
    this ILogger logger,
    Guid orderId,
    int totalPrice
  );

  [LoggerMessage(1003, LogLevel.Warning, "Couldn't find reservation for order with id: {OrderId}")]
  public static partial void CouldntFindOrderReservation(this ILogger logger, Guid orderId);

  [LoggerMessage(1004, LogLevel.Information, "Reservation removed for order with id: {OrderId}")]
  public static partial void ReservationRemoved(this ILogger logger, Guid orderId);


}
