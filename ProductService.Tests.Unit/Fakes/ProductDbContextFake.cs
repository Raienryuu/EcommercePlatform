using Microsoft.EntityFrameworkCore;
using ProductService;

namespace ProductServiceTests.Fakes;

public class ProductDbContextFake(DbContextOptions<ProductDbContext> options) : ProductDbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
	base.OnConfiguring(optionsBuilder);
	optionsBuilder.EnableSensitiveDataLogging(true);
  }
} 
