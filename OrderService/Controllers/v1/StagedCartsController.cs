using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class StagedCartsController(OrderDbContext context) : ControllerBase
{
  private readonly OrderDbContext _context = context;

  [HttpPost]
  public async Task<IActionResult> CreateStagedCart(
    [FromHeader(Name = "UserId")] Guid userId,
    StageCartRequest cart,
    CancellationToken ct
  )
  {
    var stagedCart = new StagedCart
    {
      OwnerId = userId,
      Products = cart.Products,
      ValidUntil = DateTime.UtcNow.AddHours(1),
    };

    _ = await _context.StagedCarts.AddAsync(stagedCart, ct);
    _ = await _context.SaveChangesAsync(ct);

    return Ok(stagedCart);
  }

  [HttpGet]
  public async Task<IActionResult> GetStagedCart(
    [FromHeader(Name = "UserId")] Guid userId,
    CancellationToken ct
  )
  {
    if (userId == Guid.Empty)
    {
      return BadRequest("User is not logged in.");
    }

    var cart = await _context.StagedCarts.Where(x => x.OwnerId == userId).FirstOrDefaultAsync(ct);

    if (cart is null)
    {
      return NotFound();
    }

    if (cart.ValidUntil < DateTime.UtcNow)
    {
      cart = new StagedCart
      {
        OwnerId = cart.OwnerId,
        Products = cart.Products,
        ValidUntil = DateTime.UtcNow.AddHours(1),
      };
      _ = await _context.SaveChangesAsync(ct);
    }

    return Ok(cart);
  }
}
