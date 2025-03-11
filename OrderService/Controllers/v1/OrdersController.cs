using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext context, IPublishEndpoint publisher) : ControllerBase
{
  private readonly OrderDbContext _context = context;
  private readonly IPublishEndpoint _publisher = publisher;

  // GET: Orders/all
  [HttpGet("/all")]
  public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
  {
    return Ok(await _context.Orders.ToListAsync());
  }

  [HttpGet("{orderId:Guid}")]
  public async Task<ActionResult<Order>> GetOrder(
    [FromHeader(Name = "userId")] Guid userId,
    Guid orderId,
    CancellationToken ct
  )
  {
    var order = await _context.Orders.FindAsync([orderId], cancellationToken: ct);
    return userId != order?.UserId ? BadRequest("Mismatch between logged user Id and order's user Id.")
      : order is null ? NotFound()
      : Ok(order);
  }

  [HttpGet]
  public async Task<ActionResult<List<Order>>> GetUserOrders([FromHeader(Name = "userId")] Guid userId)
  {
    var orders = await _context.Orders.Where(x => x.UserId == userId).ToListAsync();

    return Ok(orders);
  }

  [HttpPut("{orderId:Guid}")]
  public async Task<IActionResult> PutOrder(
    [FromHeader(Name = "userId")] Guid userId,
    Guid orderId,
    Order order
  )
  {
    order.OrderId = orderId;

    _context.Entry(order).State = EntityState.Modified;

    if (userId != order.UserId)
    {
      return BadRequest("Mismatch between logged user Id and order's user Id.");
    }

    _ = await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpPost]
  public async Task<ActionResult<Order>> PostOrder(
    [FromHeader(Name = "userId")] Guid userId,
    [FromBody] Order order
  )
  {
    var newOrderId = NewId.NextGuid();
    order.OrderId = newOrderId;
    order.UserId = userId;

    _ = await _context.Orders.AddAsync(order);
    _ = await _context.SaveChangesAsync();

    await _publisher.Publish<IOrderSubmitted>(new { OrderId = newOrderId, order.Products });

    return CreatedAtAction("GetOrder", new { orderId = order.OrderId }, order);
  }

  [HttpDelete("{id:Guid}")]
  public async Task<IActionResult> DeleteOrder([FromHeader(Name = "userId")] Guid userId, Guid id)
  {
    var order = await _context.Orders.FindAsync(id);

    if (order is null)
    {
      return NoContent();
    }
    if (order.UserId != userId)
    {
      return BadRequest("Mismatch between logged user Id and order's user Id.");
    }
    _ = _context.Orders.Remove(order);
    _ = await _context.SaveChangesAsync();

    return NoContent();
  }
}
