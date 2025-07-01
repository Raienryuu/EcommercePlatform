using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Utility;

internal static class ProductHelpers
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
    oldProduct.RefreshConcurrencyStamp();
  }

  public static async Task<bool> DoesCategoryExists(ProductDbContext db, int categoryId)
  {
    var result = await db.ProductCategories.FirstOrDefaultAsync(cat => cat.Id == categoryId);
    return result is not null;
  }
}
