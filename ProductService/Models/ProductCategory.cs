using Microsoft.EntityFrameworkCore;

namespace ProductService.Models
{
  [Index(nameof(CategoryName), IsUnique = true)]
  public class ProductCategory
  {
	public int Id { get; set; }
	public required string CategoryName { get; set; }
	public ProductCategory? ParentCategory { get; set; }
  }
}