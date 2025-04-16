using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Endpoints.OrderEndpoints;

public static class SetDeliveryMethodEndpoint
{
  public static WebApplication MapSetDeliveryMethodEndpoint(this WebApplication app)
  {
    app.MapPut(
        EndpointRoutes.Orders.SET_DELIVERY,
        async (
          OrderDbContext context,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId,
          Guid deliveryMethodId
        ) =>
        {
          var order = await context.Orders.FindAsync(orderId);

          if (order is null)
          {
            return Results.NotFound("No order found with given id.");
          }

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
