using Microsoft.AspNetCore.Mvc;

namespace OrderService.Endpoints.OrderEndpoints;

public static class DeleteOrderEndpoint
{
  public static WebApplication MapDeleteOrderEndpoint(this WebApplication app)
  {
    app.MapDelete(
        EndpointRoutes.Orders.DELETE_ORDER,
        async (
          OrderDbContext context,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          var order = await context.Orders.FindAsync(orderId);

          if (order is null)
          {
            return Results.NoContent();
          }
          if (order.UserId != userId)
          {
            return Results.BadRequest("Mismatch between logged user Id and order's user Id.");
          }
          _ = context.Orders.Remove(order);
          _ = await context.SaveChangesAsync();

          return Results.NoContent();
        }
      )
      .WithName(nameof(DeleteOrderEndpoint));

    return app;
  }
}
