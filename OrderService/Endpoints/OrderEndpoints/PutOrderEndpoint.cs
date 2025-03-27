using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Endpoints.OrderEndpoints;

public static class PutOrderEndpoint
{
  public static WebApplication MapPutOrderEndpoint(this WebApplication app)
  {
    app.MapPut(
        EndpointRoutes.Orders.PUT_ORDER,
        async (
          OrderDbContext context,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId,
          Order order
        ) =>
        {
          order.OrderId = orderId;

          context.Entry(order).State = EntityState.Modified;

          if (userId != order.UserId)
          {
            return Results.BadRequest("Mismatch between logged user Id and order's user Id.");
          }

          _ = await context.SaveChangesAsync();

          return Results.NoContent();
        }
      )
      .WithName(nameof(PutOrderEndpoint));
    return app;
  }
}
