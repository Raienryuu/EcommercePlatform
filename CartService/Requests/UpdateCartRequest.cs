namespace CartService.Requests;

public class UpdateCartRequest
{
  public Guid Id { get; set; }
  public required List<Product> Products { get; set; }
}
