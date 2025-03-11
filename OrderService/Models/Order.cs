using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[PrimaryKey(nameof(OrderId))]
[method: JsonConstructor]
public class Order()
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public bool IsConfirmed { get; set; }
  public OrderStatus.Type Status { get; set; }
  public string? Notes { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow;
  public DateTime LastModified { get; set; } = DateTime.UtcNow;
  public ICollection<OrderProduct> Products { get; set; } = [];
}
