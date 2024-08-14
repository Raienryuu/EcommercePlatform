using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Tests.Fakes;

public class ProductDbContextFake(DbContextOptions options) : ProductDbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
    modelBuilder.Entity<Product>().HasOne(p => p.Category)
      .WithMany()
      .HasForeignKey(p => p.CategoryId)
      .OnDelete(DeleteBehavior.Cascade)
      .IsRequired(true);

  }
}
