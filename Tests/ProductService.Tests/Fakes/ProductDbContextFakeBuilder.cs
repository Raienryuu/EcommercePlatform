using ProductService.Models;

namespace ProductService.Tests.Fakes;

public class ProductDbContextFakeBuilder
{
  private ProductDbContext _db = null!;

  public ProductDbContextFakeBuilder WithProducts()
  {
    var categoryId = _db.ProductCategories.First().Id;
    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("4471a016-f1d0-4a74-b479-26cfed9d201c"),
        CategoryId = categoryId,
        Name = "Red Cup",
        Description = "Fairly big cup",
        Price = 20,
        Quantity = 10,
      }
    );
    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("f40d0b49-e347-46f8-a127-b0c359c2ffce"),
        CategoryId = categoryId,
        Name = "White Cup",
        Description = "Fairly big cup",
        Price = 15,
        Quantity = 0,
      }
    );
    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("7211b6de-270f-49be-92b4-ac067f212f0e"),
        CategoryId = categoryId,
        Name = "Green Cup",
        Description = "Fairly big cup",
        Price = 30,
        Quantity = 50,
      }
    );
    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("f76595d5-d0a2-4c80-be79-53df49f3311b"),
        CategoryId = categoryId,
        Name = "Blue Cup",
        Description = "Fairly big cup",
        Price = 25,
        Quantity = 6,
      }
    );

    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("a1b50ba6-9a51-4f21-b89b-cc0c22494c00"),
        CategoryId = categoryId,
        Name = "Yellow Cup",
        Description = "Fairly big cup",
        Price = 23,
        Quantity = 50,
      }
    );
    _ = _db.Products.Add(
      new Product
      {
        Id = Guid.Parse("de463be0-f960-4535-b146-de79aa3cd214"),
        CategoryId = categoryId,
        Name = "T-shirt",
        Description = "One size fits all",
        Price = 1500,
        Quantity = 34,
      }
    );
    _ = _db.SaveChanges();
    return this;
  }

  public ProductDbContextFakeBuilder WithCategories()
  {
    var parent = new ProductCategory { CategoryName = "Sample category" }; // 6
    _ = _db.ProductCategories.Add(parent);
    _ = _db.ProductCategories.Add(
      new ProductCategory
      {
        // Id = 7,
        CategoryName = "Category to update",
      }
    );
    _ = _db.ProductCategories.Add(
      new ProductCategory
      {
        // Id = 8,
        CategoryName = "Category to delete",
      }
    );

    _ = _db.ProductCategories.Add(
      new ProductCategory
      {
        // Id = 9,
        CategoryName = "Child Category",
        ParentCategory = parent,
      }
    );
    _ = _db.SaveChanges();

    return this;
  }

  public ProductDbContext Build()
  {
    _ = _db.SaveChanges();
    return _db;
  }

  //public ProductDbContext BuildFromEmptyWithData(string connectionString)
  //{
  //	var options = new DbContextOptionsBuilder<ProductDbContext>()
  //		.UseSqlServer(connectionString).Options;
  //	_db = new(options);
  //	_db.Database.EnsureCreated();
  //	WithCategories();
  //	_db.SaveChanges();
  //	WithProducts();
  //	return Build();
  //}
  public void FillWithTestData(ProductDbContext db)
  {
    _db = db;
    _ = WithCategories();
    _ = WithProducts();
  }
}
