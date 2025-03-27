using CartService.Models;

namespace CartService.Responses;

public class ResponseCartDTO
{
  public required List<Product> Products { get; set; }
}
