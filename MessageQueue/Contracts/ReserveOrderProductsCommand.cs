using MassTransit;
using MessageQueue.DTOs;

namespace MessageQueue.Contracts;

[EntityName(nameof(ReserveOrderProductsCommand))]
public class ReserveOrderProductsCommand
{
  public Guid OrderId { get; set; }
  public required OrderProductDTO[] Products { get; set; }
}


