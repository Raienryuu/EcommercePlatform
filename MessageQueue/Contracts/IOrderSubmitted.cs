using MessageQueue.DTOs;

namespace MessageQueue.Contracts;

public interface IOrderSubmitted
{
  Guid OrderId { get; }
  OrderProductDTO[] Products { get; set; }
}
