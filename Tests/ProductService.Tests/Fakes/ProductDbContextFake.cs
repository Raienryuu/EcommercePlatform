using Microsoft.EntityFrameworkCore;

namespace ProductService.Tests.Fakes;

public class ProductDbContextFake(DbContextOptions options) : ProductDbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<ProductService.Models.Product>()
      .Property(p => p.Price)
      .HasConversion<double>();
  }

  // Mimic the real function for test compatibility in tests
  public new IQueryable<ProductService.Models.Product> GetProductsFromCategoryHierarchy(int categoryId)
  {
    // Recursively find all descendant category IDs
    var allCategoryIds = new HashSet<int>();
    var categories = ProductCategories.ToList();
    void AddDescendants(int id)
    {
      allCategoryIds.Add(id);
      foreach (var child in categories.Where(c => c.ParentCategory != null && c.ParentCategory.Id == id))
      {
        AddDescendants(child.Id);
      }
    }
    AddDescendants(categoryId);
    return Products.Where(p => allCategoryIds.Contains(p.CategoryId));
  }
}
