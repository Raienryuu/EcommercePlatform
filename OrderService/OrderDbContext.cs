using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService;

public class OrderDbContext : DbContext
{
  public OrderDbContext(DbContextOptions options) : base(options) { }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
	base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
	base.OnModelCreating(modelBuilder);

	modelBuilder.Entity<Order>().Property("LastModified").ValueGeneratedOnUpdate();
  }

  public DbSet<Order> Orders { get; set; }
}
