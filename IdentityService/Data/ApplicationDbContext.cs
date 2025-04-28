using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
  {
    public DbSet<UserAddress> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey("LoginProvider", "ProviderKey");
      FillWithDevelData(modelBuilder);
    }

    public static void FillWithDevelData(ModelBuilder modelBuilder)
    {
      modelBuilder
        .Entity<IdentityRole<Guid>>()
        .HasData(
          new IdentityRole<Guid>
          {
            Id = Guid.Parse("89a59295-562a-4c0b-9d52-1d3c07da4afe"),
            Name = "User",
            NormalizedName = "USER",
            ConcurrencyStamp = "123",
          }
        );
      modelBuilder
        .Entity<IdentityRole<Guid>>()
        .HasData(
          new IdentityRole<Guid>
          {
            Id = Guid.Parse("5ad84e59-945b-463a-bfa5-a10834aacb14"),
            Name = "Moderator",
            NormalizedName = "MODERATOR",
            ConcurrencyStamp = "163",
          }
        );
      modelBuilder
        .Entity<IdentityRole<Guid>>()
        .HasData(
          new IdentityRole<Guid>
          {
            Id = Guid.Parse("71d4e31e-3e7d-4194-b213-be5d4a4630af"),
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",
            ConcurrencyStamp = "163",
          }
        );
    }
  }
}
