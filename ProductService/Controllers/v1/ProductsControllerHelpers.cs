using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Models;

internal static class ProductsControllerHelpers
{
  public static async Task AssignNewValuesToProduct(
    ProductDbContext db,
    Product updatedProduct,
    Product oldProduct
  )
  {
    oldProduct.Price = updatedProduct.Price;
    oldProduct.Quantity = updatedProduct.Quantity;
    oldProduct.Name = updatedProduct.Name;
    oldProduct.Description = updatedProduct.Description;
    oldProduct.CategoryId = updatedProduct.CategoryId;
    oldProduct.Category = await db.ProductCategories.FirstAsync(cat => cat.Id == updatedProduct.CategoryId);
  }

  public static string CreateErrorResponse(string message)
  {
    return $"\"type\": \"error\", \"message\": \"{message}\"";
  }
}

