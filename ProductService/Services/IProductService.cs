using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
  Task<ServiceResult<List<Product>>> GetBatchProducts(
    List<Guid> productsIds,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<Product>> UpdateProduct(
    Product updatedProduct,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<Product>> AddProduct(Product newProduct, CancellationToken cancellationToken = default);
  Task<ServiceResult<Product>> GetProduct(Guid productId, CancellationToken cancellationToken = default);
}
