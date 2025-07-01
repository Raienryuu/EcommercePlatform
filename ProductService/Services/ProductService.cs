using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility;

namespace ProductService.Services;

public partial class ProductService(ProductDbContext db) : IProductService
{
  public async Task<ServiceResult<Product>> AddProduct(Product newProduct)
  {
    newProduct.Category = await db.ProductCategories.SingleOrDefaultAsync(c => c.Id == newProduct.CategoryId);

    if (newProduct.Category is null)
    {
      return new ErrorServiceResult<Product>(400, "Category not found");
    }
    newProduct.RefreshConcurrencyStamp();
    db.Products.Add(newProduct);
    await db.SaveChangesAsync();

    return new SuccessServiceResult<Product>(newProduct, 201);
  }

  public async Task<ServiceResult<List<Product>>> GetBatchProducts(List<Guid> productsIds)
  {
    var products = await db.Products.Where(x => productsIds.Contains(x.Id)).ToListAsync();

    if (products.Count != productsIds.Count)
      return ServiceResults.NotFoundServiceResult<List<Product>>("Some products id were not found");

    return ServiceResults.OkServiceResult(products);
  }

  public async Task<ServiceResult<Product>> UpdateProduct(Product updatedProduct)
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(p => p.Id == updatedProduct.Id);
    if (oldProduct is null)
    {
      return ServiceResults.NotFoundServiceResult<Product>("Product not found");
    }
    if (!await ProductHelpers.DoesCategoryExists(db, updatedProduct.CategoryId))
    {
      return ServiceResults.NotFoundServiceResult<Product>("Given category does not exists");
    }
    if (updatedProduct.ConcurrencyStamp != oldProduct.ConcurrencyStamp)
    {
      return new ErrorServiceResult<Product>(422, "ConcurrencyStamp mismatch");
    }

    await ProductHelpers.AssignNewValuesToProduct(db, updatedProduct, oldProduct);

    await db.SaveChangesAsync();

    return ServiceResults.OkServiceResult(oldProduct);
  }
}
