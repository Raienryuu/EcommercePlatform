using Microsoft.AspNetCore.Mvc;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetOrderEndpoint
{
  public static WebApplication MapGetOrderEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.GET_ORDER,
        async (
          OrderDbContext context,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          var order = await context.Orders.FindAsync([orderId], cancellationToken: ct);
          return userId != order?.UserId
              ? Results.BadRequest("Mismatch between logged user Id and order's user Id.")
            : order is null ? Results.NotFound()
            : Results.Ok(order);
        }
      )
      .WithName(nameof(GetOrderEndpoint));

    return app;
  }
}
