namespace MessageQueue.Contracts;

public interface IOrderCancelledPaymentIntentCancelled
{
  Guid OrderId { get; set; }
}
