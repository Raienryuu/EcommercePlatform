using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
  Task<ServiceResult<List<Product>>> GetBatchProducts(List<Guid> productsIds);
  Task<ServiceResult<Product>> UpdateProduct(Product updatedProduct);
  Task<ServiceResult<Product>> AddProduct(Product newProduct);
  Task<ServiceResult<Product>> GetProduct(Guid productId);
}
