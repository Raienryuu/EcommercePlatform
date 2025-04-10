using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
[PrimaryKey(nameof(ProductId))]
public class OrderProduct
{
  public required Guid ProductId { get; init; }
  public required int Quantity { get; init; }

  /// <summary>
  /// Price in currency smallest units (eg. cents for $)
  /// </summary>
  public required int Price { get; init; }
}
