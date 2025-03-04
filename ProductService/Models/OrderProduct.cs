using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models
{
  [NotMapped]
  public class OrderProduct
  {
    public Guid Id { get; set; }
    public required int Quantity { get; set; }
  }
}
