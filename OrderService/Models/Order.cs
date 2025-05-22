using System.Text.Json.Serialization;
using Contracts;
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
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public OrderStatus Status { get; set; } = OrderStatus.AwaitingConfirmation;
  public string? Notes { get; set; }
  public DateTime Created { get; set; } = DateTime.UtcNow;
  public DateTime LastModified { get; set; } = DateTime.UtcNow;
  public ICollection<OrderProduct> Products { get; set; } = [];
  public string? StripePaymentId { get; set; }
  public int? TotalPriceInSmallestCurrencyUnit { get; set; }
  public required string CurrencyISO { get; set; }
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public PaymentStatus PaymentStatus { get; set; }
  public OrderDelivery? Delivery { get; set; }
}
