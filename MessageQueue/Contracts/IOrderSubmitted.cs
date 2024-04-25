using MessageQueue.DTOs;

namespace MessageQueue.Contracts;
public interface IOrderSubmitted
  {
	Guid OrderId { get; }
	public OrderProductDTO[] Products { get; set; }
  }