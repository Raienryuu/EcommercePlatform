namespace MessageQueue.Contracts;

public interface IOrderCancellationRequest
{
  public Guid OrderId { get; }
}
