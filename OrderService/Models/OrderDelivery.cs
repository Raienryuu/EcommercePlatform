using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
[PrimaryKey(nameof(DeliveryId))]
public class OrderDelivery
{
  public Guid DeliveryId { get; set; }
  public required string HandlerName { get; set; }
  public string? ExternalDeliveryId { get; set; }

  [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
  public required decimal Price { get; set; }
}
