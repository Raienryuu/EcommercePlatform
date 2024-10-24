using MassTransit;
using MessageQueue.DTOs;

namespace MessageQueue.Contracts;
[EntityName(nameof(ReserveOrderProductsCommand))]
public class ReserveOrderProductsCommand
{
  public Guid OrderId { get; set; }
  public OrderProductDTO[] Products { get; set; } = [];
}