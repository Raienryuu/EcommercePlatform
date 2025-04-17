using System.ComponentModel.DataAnnotations;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[PrimaryKey(nameof(DeliveryId))]
public class Delivery
{
  public Guid DeliveryId { get; set; }
  public required string HandlerName { get; set; }
  public required string Name { get; set; }

  public required DeliveryType DeliveryType { get; set; }
  public required PaymentType PaymentType { get; set; }

  [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
  public required decimal Price { get; set; }
}
