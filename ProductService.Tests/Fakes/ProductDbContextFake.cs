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
  }
}
