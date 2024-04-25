namespace MessageQueue.Contracts;
public interface IOrderReserved
{
  Guid OrderId { get; }
}