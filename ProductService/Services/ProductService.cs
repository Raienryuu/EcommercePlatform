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
      return ServiceResults.NotFound<Product>("Category not found");
    }
    newProduct.RefreshConcurrencyStamp();
    db.Products.Add(newProduct);
    await db.SaveChangesAsync();

    return ServiceResults.Success(newProduct, 201);
  }

  public async Task<ServiceResult<List<Product>>> GetBatchProducts(List<Guid> productsIds)
  {
    var products = await db.Products.Where(x => productsIds.Contains(x.Id)).ToListAsync();

    if (products.Count != productsIds.Count)
      return ServiceResults.NotFound<List<Product>>("Some products id were not found");

    return ServiceResults.Ok(products);
  }

  public async Task<ServiceResult<Product>> GetProduct(Guid productId)
  {
    var product = await db.Products.FindAsync(productId);
    if (product is null)
    {
      return ServiceResults.NotFound<Product>($"No product found with given ID: {productId}.");
    }
    return ServiceResults.Ok(product);
  }

  public async Task<ServiceResult<Product>> UpdateProduct(Product updatedProduct)
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(p => p.Id == updatedProduct.Id);
    if (oldProduct is null)
    {
      return ServiceResults.NotFound<Product>("Product not found");
    }
    if (!await ProductHelpers.DoesCategoryExists(db, updatedProduct.CategoryId))
    {
      return ServiceResults.NotFound<Product>("Given category does not exists");
    }
    if (updatedProduct.ConcurrencyStamp != oldProduct.ConcurrencyStamp)
    {
      return new ErrorServiceResult<Product>(422, "ConcurrencyStamp mismatch");
    }

    await ProductHelpers.AssignNewValuesToProduct(db, updatedProduct, oldProduct);

    await db.SaveChangesAsync();

    return ServiceResults.Ok(oldProduct);
  }
}
