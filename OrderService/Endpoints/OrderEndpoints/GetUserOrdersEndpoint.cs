using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetUserOrdersEndpoint
{
  public static WebApplication MapGetUserOrdersEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Orders.GET_USER_ORDERS,
        async (IOrderService orderService, CancellationToken ct, [FromHeader(Name = "UserId")] Guid userId) =>
        {
          var orders = await orderService.GetUserOrders(userId, ct);
          return Results.Ok(orders);
        }
      )
      .WithName(nameof(GetUserOrdersEndpoint));

    return app;
  }
}
