using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models;

[Index(nameof(Id))]
public class OrdersReserved
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public Guid Id { get; set; }
  public bool IsReserved { get; set; }
  public DateTime ReserveTimestamp { get; set; }
}
