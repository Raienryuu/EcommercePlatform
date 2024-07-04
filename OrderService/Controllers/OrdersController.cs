using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;


namespace OrderService.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
	private readonly OrderDbContext _context;
	private readonly IPublishEndpoint _publisher;

	public OrdersController(OrderDbContext context, IPublishEndpoint publisher)
	{
	  _context = context;
	  _publisher = publisher;
	}

	// GET: Orders/all
	[HttpGet("/all")]
	public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
	{
	  return await _context.Orders.ToListAsync();
	}

	[HttpGet("{id:Guid}")]
	public async Task<ActionResult<Order>> GetOrder(Guid id)
	{
	  var order = await _context.Orders.FindAsync(id);

	  if (order == null)
	  {
		return NotFound();
	  }

	  return order;
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<List<Order>>> GetUserOrders(int id)
	{
	  var orders = await _context.Orders
		.Where(x => x.UserId == id)
		.ToListAsync();

	  return orders;
	}

	[HttpPut("{id:Guid}")]
	public async Task<IActionResult> PutOrder(Guid id, Order order)
	{
	  if (id != order.OrderId)
	  {
		return BadRequest();
	  }

	  _context.Entry(order).State = EntityState.Modified;

	  try
	  {
		await _context.SaveChangesAsync();
	  }
	  catch (DbUpdateConcurrencyException)
	  {
		if (!OrderExists(id))
		{
		  return NotFound();
		}
		else
		{
		  throw;
		}
	  }

	  return NoContent();
	}

	[HttpPost]
	public async Task<ActionResult<Order>> PostOrder(Order order)
	{
	  var newId = NewId.NextGuid();
	  order.OrderId = newId;

	  await _context.Orders.AddAsync(order);
	  await _context.SaveChangesAsync();

	  await _publisher.Publish<IOrderSubmitted>(new
	  {
		OrderId = newId,
		order.Products,
	  });

	  return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
	}

	[HttpDelete("{id:Guid}")]
	public async Task<IActionResult> DeleteOrder(Guid id)
	{
	  var order = await _context.Orders.FindAsync(id);
	  if (order == null)
	  {
		return NotFound();
	  }

	  _context.Orders.Remove(order);
	  await _context.SaveChangesAsync();

	  return NoContent();
	}

	private bool OrderExists(Guid id)
	{
	  return _context.Orders.Any(e => e.OrderId == id);
	}
  }
}