using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Services;

public class ProductCategoryService(ProductDbContext context) : IProductCategoryService
{
  private readonly ProductDbContext _context = context;

  public async Task<ServiceResult<List<ProductCategory>>> GetProductCategories(CancellationToken ct)
  {
    var categories = await _context.ProductCategories.AsNoTracking().ToListAsync(cancellationToken: ct);
    return ServiceResults.Ok(categories);
  }

  public async Task<ServiceResult<ProductCategory>> GetProductCategory(int id, CancellationToken ct)
  {
    var productCategory = await _context
      .ProductCategories.AsNoTracking()
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync(cancellationToken: ct);

    if (productCategory == null)
    {
      return ServiceResults.NotFound<ProductCategory>("No category found with a given ID.");
    }

    return ServiceResults.Ok(productCategory);
  }

  public async Task<ServiceResult<List<ProductCategory>>> GetChildrenCategories(int id, CancellationToken ct)
  {
    var productCategory = await _context
      .ProductCategories.AsNoTracking()
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync(cancellationToken: ct);

    if (productCategory == null)
    {
      return ServiceResults.NotFound<List<ProductCategory>>("No category found with a given ID.");
    }

    var releatedChildCategories = await _context
      .ProductCategories.AsNoTracking()
      .Where(x => x.ParentCategory != null && x.ParentCategory.Id == productCategory.Id)
      .ToListAsync(cancellationToken: ct);

    return ServiceResults.Ok(releatedChildCategories);
  }

  public async Task<ServiceResult> UpdateProductCategory(
    int id,
    ProductCategory productCategory,
    CancellationToken ct
  )
  {
    if (id != productCategory.Id)
    {
      return ServiceResults.Error("IDs do not match.", System.Net.HttpStatusCode.BadRequest);
    }

    if (!await ProductCategoryExists(id, ct))
    {
      return ServiceResults.Error("No category found with a given ID.", System.Net.HttpStatusCode.NotFound);
    }

    if (!await AssignParent(productCategory, ct))
    {
      return ServiceResults.Error("Parent category not found.", System.Net.HttpStatusCode.NotFound);
    }

    try
    {
      _context.Entry(productCategory).State = EntityState.Modified;
      await _context.SaveChangesAsync(ct);
      _context.Entry(productCategory).State = EntityState.Detached;
    }
    catch (DbUpdateConcurrencyException)
    {
      return ServiceResults.Error("Concurrency error occurred. Try again using newest data.", System.Net.HttpStatusCode.InternalServerError);
    }

    return ServiceResults.Success(System.Net.HttpStatusCode.NoContent);
  }

  public async Task<ServiceResult<ProductCategory>> CreateProductCategory(
    ProductCategory productCategory,
    CancellationToken ct
  )
  {
    var existingCategory = await _context
      .ProductCategories.Where(_ => _.CategoryName == productCategory.CategoryName)
      .FirstOrDefaultAsync(cancellationToken: ct);

    if (existingCategory is not null)
    {
      return ServiceResults.Error("Category already exists.", System.Net.HttpStatusCode.Conflict); // Conflict
    }

    if (!await AssignParent(productCategory, ct))
    {
      return ServiceResults.Error("Parent category not found.", System.Net.HttpStatusCode.BadRequest); // Bad Request
    }

    _context.ProductCategories.Add(productCategory);
    await _context.SaveChangesAsync(ct);
    _context.Entry(productCategory).State = EntityState.Detached;

    return ServiceResults.Success(productCategory, System.Net.HttpStatusCode.Created); // Created
  }

  public async Task<ServiceResult> DeleteProductCategory(int id, CancellationToken ct)
  {
    var productCategory = await _context.ProductCategories.FindAsync([id], cancellationToken: ct);
    if (productCategory == null)
    {
      return ServiceResults.Error("No category found with a given ID.", System.Net.HttpStatusCode.NotFound);
    }

    _context.ProductCategories.Remove(productCategory);
    await _context.SaveChangesAsync(ct);

    return ServiceResults.Success(System.Net.HttpStatusCode.NoContent); // No Content
  }

  private Task<bool> ProductCategoryExists(int id, CancellationToken ct)
  {
    return _context.ProductCategories.AnyAsync(e => e.Id == id, cancellationToken: ct);
  }

  private async Task<bool> AssignParent(ProductCategory productCategory, CancellationToken ct)
  {
    if (productCategory.ParentCategory is null)
      return true;

    var parent = await _context.ProductCategories.FirstOrDefaultAsync(
      x => x.Id == productCategory.ParentCategory.Id,
      cancellationToken: ct
    );

    if (parent?.CategoryName == productCategory.ParentCategory.CategoryName)
    {
      productCategory.ParentCategory = parent;
      return true;
    }
    return false;
  }
}
