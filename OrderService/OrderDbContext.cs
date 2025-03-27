using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService;

public class OrderDbContext(DbContextOptions options) : DbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    _ = modelBuilder.Entity<Order>().Property("LastModified").ValueGeneratedOnUpdate();
  }

  public required DbSet<Order> Orders { get; set; }
}
