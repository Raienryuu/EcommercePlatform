namespace OrderService.Models;

public class StageCartRequest
{
  public Guid Id { get; init; }
  public required List<OrderProduct> Products { get; init; }
}
