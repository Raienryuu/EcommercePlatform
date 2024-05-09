using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Models
{
  [PrimaryKey(nameof(OrderId))]
  public class Order
  {
	public Guid OrderId { get; set; }
	[Required]
	public int UserId { get; set; }
	public bool IsConfirmed { get; set; }
	public OrderStatus.Type Status { get; set; }
	public string? Notes { get; set; }
	public DateTime Created { get; set; } = DateTime.UtcNow;
	public DateTime LastModified { get; set; } = DateTime.UtcNow;
	public ICollection<OrderProduct> Products { get; set; } = [];

	[JsonConstructor]
	public Order()
	{
	  LastModified = DateTime.UtcNow;
	}
  }
}
