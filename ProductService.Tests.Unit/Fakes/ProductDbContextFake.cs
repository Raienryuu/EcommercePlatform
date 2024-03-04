using Microsoft.EntityFrameworkCore;
using ProductService;

namespace ProductServiceTests.Fakes;

public class ProductDbContextFake : ProductDbContext
{
  public ProductDbContextFake() : base(new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(databaseName: $"ProductsTest-{Guid.NewGuid()}")
        .Options) {}
} 
