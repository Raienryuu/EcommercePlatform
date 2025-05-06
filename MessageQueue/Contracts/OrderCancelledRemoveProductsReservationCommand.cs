using MassTransit;

namespace MessageQueue.Contracts;

[EntityName(nameof(OrderCancelledRemoveProductsReservationCommand))]
public class OrderCancelledRemoveProductsReservationCommand
{
  public Guid OrderId { get; set; }
}
