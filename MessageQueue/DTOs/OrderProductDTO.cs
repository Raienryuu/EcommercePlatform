namespace MessageQueue.DTOs;

public class OrderProductDTO
{
  public required Guid ProductId { get; set; }
  public required int Quantity { get; set; }
}
