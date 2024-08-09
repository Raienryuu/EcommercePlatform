using Microsoft.EntityFrameworkCore;
using ProductService;

namespace ProductService.Tests.Fakes;

public class ProductDbContextFake(DbContextOptions<ProductDbContext> options) : ProductDbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
	base.OnConfiguring(optionsBuilder);
	optionsBuilder.EnableSensitiveDataLogging(true);
  }
} 
