namespace MessageQueue.DTOs;

public class OrderProductDTO
{
  public Guid ProductId { get; set; }
  public required int Quantity { get; set; }
}

