using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
  [NotMapped]
  public class OrderProduct
  {
	public int Id { get; set; }
	public required int Quantity { get; set; }
  }
}