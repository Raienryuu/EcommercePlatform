using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CancelOrderEndpoint
{
  public static WebApplication MapCancelOrderEndpoint(this WebApplication app)
  {
    app.MapDelete(
        EndpointRoutes.Orders.CANCEL_ORDER,
        async (
          IOrderService orderService,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          var (isSuccess, error) = await orderService.CancelOrder(orderId, userId, ct);
          return isSuccess ? Results.NoContent() : Results.BadRequest(error);
        }
      )
      .WithName(nameof(CancelOrderEndpoint));

    return app;
  }
}
