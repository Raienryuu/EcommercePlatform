using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService;

public class ProductDbContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<Product> Products { get; set; }
  public DbSet<ProductCategory> ProductCategories { get; set; }
  public DbSet<OrderReserved> OrdersReserved { get; set; }
  public DbSet<Delivery> Deliveries { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    _ = modelBuilder.Entity<Product>().Property(static p => p.Price).HasColumnType("decimal(18,2)");
    _ = modelBuilder.Entity<Delivery>().Property(static p => p.Price).HasColumnType("decimal(18,2)");
    _ = modelBuilder
      .Entity<Product>()
      .HasOne(static p => p.Category)
      .WithMany()
      .HasForeignKey(static p => p.CategoryId)
      .OnDelete(DeleteBehavior.Cascade)
      .IsRequired(true);

    _ = modelBuilder
      .Entity<Delivery>()
      .Property(d => d.PaymentType)
      .HasConversion(c => c.ToString(), c => Enum.Parse<PaymentType>(c));

    _ = modelBuilder
      .Entity<Delivery>()
      .Property(d => d.DeliveryType)
      .HasConversion(c => c.ToString(), c => Enum.Parse<DeliveryType>(c));

    FillWithDevelData(modelBuilder);
  }

  private static void FillWithDevelData(ModelBuilder modelBuilder)
  {
    _ = modelBuilder
      .Entity<ProductCategory>()
      .HasData(
        new ProductCategory
        {
          Id = 1,
          CategoryName = "Mugs",
          ParentCategory = null,
        },
        new ProductCategory()
        {
          ParentCategory = null,
          Id = 2,
          CategoryName = "Tableware",
        },
        new ProductCategory()
        {
          ParentCategory = null,
          Id = 3,
          CategoryName = "Cups",
        },
        new ProductCategory
        {
          Id = 4,
          CategoryName = "Electronics",
          ParentCategory = null,
        },
        new ProductCategory
        {
          Id = 5,
          CategoryName = "Laptops",
          ParentCategory = null,
        }
      );
    _ = modelBuilder
      .Entity<Product>()
      .HasData(
        new Product
        {
          Id = Guid.Parse("87817c15-d25f-4621-9135-2e7851b484b3"), //1
          Name = "Mug",
          Description = "White mug",
          Price = 10,
          Quantity = 100,
          CategoryId = 2,
        },
        new Product
        {
          Id = Guid.Parse("22ea176b-ea99-445f-97b3-c1afa5585562"), //2
          Name = "Cpu",
          Description = "White cup",
          Price = 10,
          Quantity = 100,
          CategoryId = 3,
        },
        new Product
        {
          Id = Guid.Parse("f12e47a0-82f9-4231-abce-63280e7d3d99"), //3
          Name = "MacBook Air M2",
          Description = "Apple laptop",
          Price = 1000,
          Quantity = 100,
          CategoryId = 4,
        },
        new Product
        {
          Id = Guid.Parse("d8f65542-f0e7-4ca7-b9c1-002898cdc379"), //4
          Name = "Lenovo ThindPad E14",
          Description = "Apple laptop",
          Price = 1000,
          Quantity = 100,
          CategoryId = 4,
        }
      );

    _ = modelBuilder
      .Entity<Delivery>()
      .HasData(
        new Delivery
        {
          DeliveryId = Guid.Parse("4e627d51-4510-4567-aee6-3830a25e040c"),
          Name = "DHL Parcel Locker",
          Price = 0m,
          DeliveryType = DeliveryType.DeliveryPoint,
          PaymentType = PaymentType.Online,
        },
        new Delivery
        {
          DeliveryId = Guid.Parse("dd6b0c88-538a-4ea2-877b-6143fab14ca5"),
          Name = "Standard shipping",
          Price = 9m,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          PaymentType = PaymentType.Online,
        },
        new Delivery
        {
          DeliveryId = Guid.Parse("8c68b176-1401-4373-aed8-3bad2f7c0f29"),
          Name = "Premium shipping",
          Price = 18m,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          PaymentType = PaymentType.Online,
        },
        new Delivery
        {
          DeliveryId = Guid.Parse("b532c6c3-0696-4536-98a5-f1dcdf4df954"),
          Name = "Standard shipping (cash)",
          Price = 20m,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          PaymentType = PaymentType.Cash,
        }
      );
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
    _ = optionsBuilder.EnableSensitiveDataLogging(false);
  }
}
