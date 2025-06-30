using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Services;

public partial class ProductService(ProductDbContext db) : IProductService
{
  public async Task<ServiceResult<List<Product>>> GetBatchProducts(List<Guid> productsIds)
  {
    var products = await db.Products.Where(x => productsIds.Contains(x.Id)).ToListAsync();

    if (products.Count != productsIds.Count)
      return ServiceResults.NotFoundServiceResult<List<Product>>("Some products Id were not found");

    return ServiceResults.OkServiceResult<List<Product>>(products);
  }
}
