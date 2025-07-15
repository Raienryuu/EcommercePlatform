using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductCategoryService
{
  Task<ServiceResult<List<ProductCategory>>> GetProductCategories(
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<ProductCategory>> GetProductCategory(
    int id,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<List<ProductCategory>>> GetChildrenCategories(
    int id,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> UpdateProductCategory(
    int id,
    ProductCategory productCategory,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<ProductCategory>> CreateProductCategory(
    ProductCategory productCategory,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> DeleteProductCategory(int id, CancellationToken cancellationToken = default);
}
