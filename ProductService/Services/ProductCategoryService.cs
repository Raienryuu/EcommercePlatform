using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Services;

public class ProductCategoryService(ProductDbContext context) : IProductCategoryService
{
  private readonly ProductDbContext _context = context;

  public async Task<ServiceResult<List<ProductCategory>>> GetProductCategories()
  {
    var categories = await _context.ProductCategories.ToListAsync();
    return ServiceResults.Ok(categories);
  }

  public async Task<ServiceResult<ProductCategory>> GetProductCategory(int id)
  {
    var productCategory = await _context.ProductCategories.FindAsync(id);

    if (productCategory == null)
    {
      return ServiceResults.NotFound<ProductCategory>("No category found with a given ID.");
    }

    return ServiceResults.Ok(productCategory);
  }

  public async Task<ServiceResult<List<ProductCategory>>> GetChildrenCategories(int id)
  {
    var productCategory = await _context.ProductCategories.Where(x => x.Id == id).FirstOrDefaultAsync();

    if (productCategory == null)
    {
      return ServiceResults.NotFound<List<ProductCategory>>("No category found with a given ID.");
    }

    var releatedChildCategories = await _context
      .ProductCategories.AsNoTracking()
      .Where(x => x.ParentCategory != null && x.ParentCategory.Id == productCategory.Id)
      .ToListAsync();

    return ServiceResults.Ok(releatedChildCategories);
  }

  public async Task<ServiceResult> UpdateProductCategory(int id, ProductCategory productCategory)
  {
    if (id != productCategory.Id)
    {
      return ServiceResults.Error("IDs do not match.", 400);
    }

    if (!await ProductCategoryExists(id))
    {
      return ServiceResults.Error("No category found with a given ID.", 404);
    }

    if (!await AssignParent(productCategory))
    {
      return ServiceResults.Error("Parent category not found.", 404);
    }

    try
    {
      _context.Entry(productCategory).State = EntityState.Modified;
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      return ServiceResults.Error("Concurrency error occurred. Try again using newest data.", 500);
    }

    return ServiceResults.Success(204);
  }

  public async Task<ServiceResult<ProductCategory>> CreateProductCategory(ProductCategory productCategory)
  {
    var existingCategory = await _context
      .ProductCategories.Where(_ => _.CategoryName == productCategory.CategoryName)
      .FirstOrDefaultAsync();

    if (existingCategory is not null)
    {
      return ServiceResults.Error<ProductCategory>("Category already exists.", 409); // Conflict
    }

    if (!await AssignParent(productCategory))
    {
      return ServiceResults.Error<ProductCategory>("Parent category not found.", 400); // Bad Request
    }

    _context.ProductCategories.Add(productCategory);
    await _context.SaveChangesAsync();

    return ServiceResults.Success(productCategory, 201); // Created
  }

  public async Task<ServiceResult> DeleteProductCategory(int id)
  {
    var productCategory = await _context.ProductCategories.FindAsync(id);
    if (productCategory == null)
    {
      return ServiceResults.Error("No category found with a given ID.", 404);
    }

    _context.ProductCategories.Remove(productCategory);
    await _context.SaveChangesAsync();

    return ServiceResults.Success(204); // No Content
  }

  private Task<bool> ProductCategoryExists(int id)
  {
    return _context.ProductCategories.AnyAsync(e => e.Id == id);
  }

  private async Task<bool> AssignParent(ProductCategory productCategory)
  {
    if (productCategory.ParentCategory is null)
      return true;

    var parent = await _context.ProductCategories.FirstOrDefaultAsync(x =>
      x.Id == productCategory.ParentCategory.Id
    );

    if (parent?.CategoryName == productCategory.ParentCategory.CategoryName)
    {
      productCategory.ParentCategory = parent;
      return true;
    }
    return false;
  }
}
