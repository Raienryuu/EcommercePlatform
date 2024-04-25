using Microsoft.EntityFrameworkCore;
using Orders.Models;
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
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
	UpdateLastModifiedOnModifiedOrders();

	return base.SaveChangesAsync(cancellationToken);
  }

  private void UpdateLastModifiedOnModifiedOrders()
  {
	foreach (var order in ChangeTracker.Entries())
	  if (order.State == EntityState.Modified)
		order.Property("LastModified").CurrentValue = DateTime.UtcNow;
  }

  public DbSet<Order> Orders { get; set; }
}
