using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Index(nameof(Price))]
[Index(nameof(Quantity))]
public class Product
{
  public Guid Id { get; set; }

  [ForeignKey("ProductCategory")]
  public required int CategoryId { get; set; }
  public virtual ProductCategory? Category { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }

  [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
  public required decimal Price { get; set; }

  [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
  public required int Quantity { get; set; }

  [ConcurrencyCheck]
  public int ConcurrencyStamp { get; set; }

  public void RefreshConcurrencyStamp()
  {
    ConcurrencyStamp += 1;
  }
}
