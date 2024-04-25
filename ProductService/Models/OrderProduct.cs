using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Orders.Models
{
  [NotMapped]
  public class OrderProduct
  {
	public int Id { get; set; }
	public required int Quantity { get; set; }
  }
}