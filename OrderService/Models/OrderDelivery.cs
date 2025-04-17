using System.ComponentModel.DataAnnotations;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Owned]
public class OrderDelivery
{
  public Guid DeliveryId { get; set; }
  public required string HandlerName { get; set; }
  public required string Name { get; set; }
  public required DeliveryType DeliveryType { get; set; }
  public required PaymentType PaymentType { get; set; }
  public string? ExternalDeliveryId { get; set; }

  [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
  public required decimal Price { get; set; }
}
