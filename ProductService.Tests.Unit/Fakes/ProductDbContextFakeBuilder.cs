using Microsoft.AspNetCore.Authorization.Infrastructure;
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
      Name = "Cup",
      Description = "Fairly big cup",
      Price = 15,
      Quantity = 10
    });
    _db.Products.Add(new Product
    {
      CategoryId = 1,
      Name = "T-shirt",
      Description = "One size fits all",
      Price = 1500,
      Quantity = 3
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