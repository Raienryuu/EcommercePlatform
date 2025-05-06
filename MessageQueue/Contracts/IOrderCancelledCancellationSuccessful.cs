
namespace MessageQueue.Contracts;

public interface IOrderCancelledCancellationSuccessful
{
  Guid OrderId { get; }
}
