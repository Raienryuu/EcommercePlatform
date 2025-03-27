using OrderService.Models;

namespace OrderService.Endpoints.Requests;

public class CreateOrderRequest
{
  public string? Notes { get; init; }
  public required List<OrderProduct> Products { get; init; }
  public required string CurrencyISO { get; init; }
}
