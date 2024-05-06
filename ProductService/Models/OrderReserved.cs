using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models;

[PrimaryKey(nameof(OrderId))]
public class OrderReserved
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public Guid OrderId { get; set; }
  public bool IsReserved { get; set; }
  public DateTime ReserveTimestamp { get; set; }
}
