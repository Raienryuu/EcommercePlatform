using CartService.Models;
using CartService.Requests;

namespace CartService.Mappers;

public static class CartMappingExtensions
{
  public static Cart ToCart(this CreateNewCartRequest dto)
  {
    return new Cart() { Products = dto.Products };
  }

  public static Cart ToCart(this UpdateCartRequest dto)
  {
    return new Cart() { Products = dto.Products };
  }
}
