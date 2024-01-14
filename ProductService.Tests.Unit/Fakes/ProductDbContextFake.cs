using Microsoft.EntityFrameworkCore;

namespace ProductServiceTests.Fakes;

public class ProductDbContextFake : ProductDbContext
{
  public ProductDbContextFake() : base(new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(databaseName: $"ProductsTest-{Guid.NewGuid()}")
        .Options) {}
} 
