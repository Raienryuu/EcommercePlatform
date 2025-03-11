using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[PrimaryKey("OwnerId")]
public class StagedCart
{
  public Guid OwnerId { get; init; }
  public required List<OrderProduct> Products { get; init; }
  public required DateTime ValidUntil { get; init; }
}
