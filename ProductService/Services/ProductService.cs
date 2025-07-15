using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility;

namespace ProductService.Services;

public partial class ProductService(ProductDbContext db) : IProductService
{
  public async Task<ServiceResult<Product>> AddProduct(
    Product newProduct,
    CancellationToken cancellationToken = default
  )
  {
    newProduct.Category = await db.ProductCategories.SingleOrDefaultAsync(
      c => c.Id == newProduct.CategoryId,
      cancellationToken
    );

    if (newProduct.Category is null)
    {
      return ServiceResults.NotFound<Product>("Category not found");
    }
    newProduct.RefreshConcurrencyStamp();
    db.Products.Add(newProduct);
    await db.SaveChangesAsync(cancellationToken);

    return ServiceResults.Success(newProduct, 201);
  }

  public async Task<ServiceResult<List<Product>>> GetBatchProducts(
    List<Guid> productsIds,
    CancellationToken cancellationToken = default
  )
  {
    var products = await db.Products.Where(x => productsIds.Contains(x.Id)).ToListAsync(cancellationToken);

    if (products.Count != productsIds.Count)
      return ServiceResults.NotFound<List<Product>>("Some products id were not found");

    return ServiceResults.Ok(products);
  }

  public async Task<ServiceResult<Product>> GetProduct(
    Guid productId,
    CancellationToken cancellationToken = default
  )
  {
    var product = await db.Products.FindAsync([productId], cancellationToken: cancellationToken);
    if (product is null)
    {
      return ServiceResults.NotFound<Product>($"No product found with given ID: {productId}.");
    }
    return ServiceResults.Ok(product);
  }

  public async Task<ServiceResult<Product>> UpdateProduct(
    Product updatedProduct,
    CancellationToken cancellationToken = default
  )
  {
    var oldProduct = await db.Products.SingleOrDefaultAsync(
      p => p.Id == updatedProduct.Id,
      cancellationToken
    );
    if (oldProduct is null)
    {
      return ServiceResults.NotFound<Product>("Product not found");
    }
    if (!await ProductHelpers.DoesCategoryExists(db, updatedProduct.CategoryId, cancellationToken))
    {
      return ServiceResults.NotFound<Product>("Given category does not exists");
    }
    if (updatedProduct.ConcurrencyStamp != oldProduct.ConcurrencyStamp)
    {
      return new ErrorServiceResult<Product>(422, "ConcurrencyStamp mismatch");
    }

    await ProductHelpers.AssignNewValuesToProduct(db, updatedProduct, oldProduct, cancellationToken);

    await db.SaveChangesAsync(cancellationToken);

    return ServiceResults.Ok(oldProduct);
  }
}
