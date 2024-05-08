using System.Diagnostics;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService;
using MessageQueue.DTOs;
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

		// GET: api/Orders
		[HttpGet("/all")]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
		{
			return await _context.Orders.ToListAsync();
		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrder(Guid id)
		{
			var order = await _context.Orders.FindAsync(id);

			if (order == null)
			{
				return NotFound();
			}

			return order;
		}

		// PUT: api/Orders/5
		[HttpPut("{id}")]
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

		// POST: api/Orders
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Order>> PostOrder(Order order)
		{
			var newId = NewId.NextGuid();
			order.OrderId = newId;

			_context.Orders.Add(order);
			await _context.SaveChangesAsync();
			
			await _publisher.Publish<IOrderSubmitted>(new
			{
				OrderId = newId,
				order.Products,
			});

			return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
		}

		// DELETE: api/Orders/5
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteOrder(int id)
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