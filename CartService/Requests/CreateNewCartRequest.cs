namespace CartService.Requests;

public class CreateNewCartRequest
{
  public required List<Product> Products { get; set; }
}
