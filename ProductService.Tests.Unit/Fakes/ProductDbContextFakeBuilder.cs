using ProductService.Models;
using ProductServiceTests.Fakes;

public class ProductDbContextFakeBuilder : IDisposable
{
  private readonly ProductDbContextFake _db = new();

  public ProductDbContextFakeBuilder WithProducts()
  {
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "Red Cup",
      Description = "Fairly big cup",
      Price = 20,
      Quantity = 10,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "White Cup",
      Description = "Fairly big cup",
      Price = 15,
      Quantity = 0,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "Green Cup",
      Description = "Fairly big cup",
      Price = 30,
      Quantity = 50,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "Blue Cup",
      Description = "Fairly big cup",
      Price = 25,
      Quantity = 6,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "Yellow Cup",
      Description = "Fairly big cup",
      Price = 23,
      Quantity = 50,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "T-shirt",
      Description = "One size fits all",
      Price = 1500,
      Quantity = 34,
      ConcurrencyStamp = Guid.NewGuid().ToByteArray()[..4]
    });
    return this;
  }

  public ProductDbContextFakeBuilder WithCategories()
  {
    _db.ProductCategories.Add(new ProductCategory
    {
      CategoryName = "Tableware"
    });
    _db.ProductCategories.Add(new ProductCategory
    {
      CategoryName = "Clothing"
    });
    _db.ProductCategories.Add(new ProductCategory
    {
      CategoryName = "Sample category"
    });
    return this;
  }

  public ProductDbContextFake Build()
  {
    _db.SaveChanges();
    return _db;
  }
  public void Dispose()
  {
    _db.Database.EnsureDeleted();
    _db.Dispose();
  }
}