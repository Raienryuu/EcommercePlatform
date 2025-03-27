namespace MessageQueue.Contracts;

public interface IOrderSubmitted
{
  Guid OrderId { get; }
}
