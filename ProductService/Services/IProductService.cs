using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductService
{
  Task<ServiceResult<List<Product>>> GetBatchProducts(List<Guid> productsIds);
}
