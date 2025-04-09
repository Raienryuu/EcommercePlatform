using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[PrimaryKey(nameof(OrderId))]
[method: JsonConstructor]
public class Order()
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public bool IsConfirmed { get; set; } = false;
  public bool IsCancelled { get; set; } = false;
  public OrderStatus.Type Status { get; set; } = OrderStatus.Type.AwaitingConfirmation;
  public string? Notes { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow;
  public DateTime LastModified { get; set; } = DateTime.UtcNow;
  public ICollection<OrderProduct> Products { get; set; } = [];
  public string? StripePaymentId { get; set; }
  public int? TotalPriceInSmallestCurrencyUnit { get; set; }
  public required string CurrencyISO { get; set; }
  public bool PaymentSucceded { get; set; } = false;
}
