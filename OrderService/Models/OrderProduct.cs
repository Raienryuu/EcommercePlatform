using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
public class OrderProduct
{
  public required Guid ProductId { get; set; }
  public required int Quantity { get; set; }
}
