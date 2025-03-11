using CartService.Models;

namespace CartService.Helpers;

public static class CartHelper
{
  public static Cart MergeCart(Cart c)
  {
    var mergedCart = new Cart() { Products = [] };
    foreach (var product in c.Products.DistinctBy(p => p.Id))
    {
      var repeatedProducts = c.Products.FindAll(p => p.Id == product.Id);
      if (repeatedProducts.Count == 1)
      {
        mergedCart.Products.Add(repeatedProducts.First());
        continue;
      }

      var totalQuanity = repeatedProducts.Sum(p => p.Amount);
      mergedCart.Products.Add(new() { Id = product.Id, Amount = totalQuanity });
    }

    return mergedCart;
  }
}
