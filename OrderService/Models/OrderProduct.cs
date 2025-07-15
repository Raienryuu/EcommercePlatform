using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
public class OrderProduct
{
  public int Id { get; set; }
  public required Guid ProductId { get; init; }
  public required int Quantity { get; init; }

  /// <summary>
  /// Price in currency smallest units (eg. cents for $)
  /// </summary>
  public required int Price { get; init; }
}
