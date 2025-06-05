using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetOrderEndpoint
{
  public static WebApplication MapGetOrderEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Orders.GET_ORDER,
        async (
          IOrderService orderService,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          var (order, error) = await orderService.GetOrder(orderId, userId, ct);

          return order is null ? Results.BadRequest(error) : Results.Ok(order);
        }
      )
      .WithName(nameof(GetOrderEndpoint));

    return app;
  }
}
