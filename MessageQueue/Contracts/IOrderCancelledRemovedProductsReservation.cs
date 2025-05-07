namespace MessageQueue.Contracts;

public interface IOrderCancelledRemovedProductsReservation
{
  public Guid OrderId { get; set; }
}
