
namespace MessageQueue.Contracts;

public interface IOrderCancelledCancellationUnavailable
{
  Guid OrderId { get; }
}
