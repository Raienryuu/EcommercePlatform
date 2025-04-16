using MessageQueue.DTOs;

namespace MessageQueue.Contracts;

public interface IOrderCreatedByUser
{
  public Guid OrderId { get; }
  public OrderProductDTO[] Products { get; set; }
  public string CurrencyISO { get; set; }
}
