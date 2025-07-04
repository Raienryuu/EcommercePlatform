using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IProductCategoryService
{
  Task<ServiceResult<List<ProductCategory>>> GetProductCategories();
  Task<ServiceResult<ProductCategory>> GetProductCategory(int id);
  Task<ServiceResult<List<ProductCategory>>> GetChildrenCategories(int id);
  Task<ServiceResult> UpdateProductCategory(int id, ProductCategory productCategory);
  Task<ServiceResult<ProductCategory>> CreateProductCategory(ProductCategory productCategory);
  Task<ServiceResult> DeleteProductCategory(int id);
}
