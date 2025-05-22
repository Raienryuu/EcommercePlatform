using Contracts;
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
    _ = modelBuilder.Entity<Order>().OwnsOne(x => x.Delivery).Property("Price").HasColumnType("money");
    _ = modelBuilder
      .Entity<Order>()
      .OwnsOne(x => x.Delivery)
      .Property(d => d.PaymentType)
      .HasConversion(c => c.ToString(), c => Enum.Parse<PaymentType>(c));

    _ = modelBuilder
      .Entity<Order>()
      .OwnsOne(x => x.Delivery)
      .Property(d => d.DeliveryType)
      .HasConversion(c => c.ToString(), c => Enum.Parse<DeliveryType>(c));

    _ = modelBuilder
      .Entity<Order>()
      .Property(d => d.PaymentStatus)
      .HasConversion(c => c.ToString(), c => Enum.Parse<PaymentStatus>(c));

    _ = modelBuilder
      .Entity<Order>()
      .Property(d => d.Status)
      .HasConversion(c => c.ToString(), c => Enum.Parse<OrderStatus>(c));
  }

  public required DbSet<Order> Orders { get; set; }
}
