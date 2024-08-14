using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService;

public class ProductDbContext : DbContext
{
	public ProductDbContext(DbContextOptions options) : base(options) { }
	public DbSet<Product> Products { get; set; }
	public DbSet<ProductCategory> ProductCategories { get; set; }
	public DbSet<OrderReserved> OrdersReserved { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
		modelBuilder.Entity<Product>().HasOne(p => p.Category)
			.WithMany()
			.HasForeignKey(p => p.CategoryId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired(true);

		FillWithDevelData(modelBuilder);
	}

	private static void FillWithDevelData(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProductCategory>().HasData(
			new ProductCategory { Id = 1, CategoryName = "Mugs", ParentCategory = null },
			new ProductCategory() { ParentCategory = null, Id = 2, CategoryName = "Tableware" },
			new ProductCategory() { ParentCategory = null, Id = 3, CategoryName = "Cups" },
			new ProductCategory { Id = 4, CategoryName = "Electronics", ParentCategory = null },
			new ProductCategory { Id = 5, CategoryName = "Laptops", ParentCategory = null }
			);
		modelBuilder.Entity<Product>().HasData(
		new Product
		{
			Id = 1,
			Name = "Mug",
			Description = "White mug",
			Price = 10,
			Quantity = 100,
			CategoryId = 2
		},
		new Product
		{
			Id = 2,
			Name = "Cpu",
			Description = "White cup",
			Price = 10,
			Quantity = 100,
			CategoryId = 3
		},
		new Product
		{
			Id = 3,
			Name = "MacBook Air M2",
			Description = "Apple laptop",
			Price = 1000,
			Quantity = 100,
			CategoryId = 4
		},
		new Product
		{
			Id = 4,
			Name = "Lenovo ThindPad E14",
			Description = "Apple laptop",
			Price = 1000,
			Quantity = 100,
			CategoryId = 4
		});
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.EnableSensitiveDataLogging(true);
	}
}