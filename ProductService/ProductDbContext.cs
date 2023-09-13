using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProductService.Models;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, CategoryName = "Tableware", ParentCategory = null },
            new ProductCategory { Id = 2, CategoryName = "Electronics", ParentCategory = null }
        );

        modelBuilder.Entity<Product>().HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Mug",
                Description = "White mug",
                Price = 10,
                Quantity = 100,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Laptop",
                Description = "Apple laptop",
                Price = 1000,
                Quantity = 100,
                CategoryId = 2
            });

    }
}