using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class SetDeliveryMethodEndpoint
{
  public static WebApplication MapSetDeliveryMethodEndpoint(this WebApplication app)
  {
    _ = app.MapPatch(
        EndpointRoutes.Orders.SET_DELIVERY,
        async (
          IOrderService orderService,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId,
          OrderDelivery deliveryMethod,
          CancellationToken ct
        ) =>
        {
          var (isSuccess, errorDetails) = await orderService.SetDeliveryMethod(
            orderId,
            userId,
            deliveryMethod,
            ct
          );

          return isSuccess ? Results.NoContent() : Results.BadRequest(errorDetails);
        }
      )
      .WithName(nameof(SetDeliveryMethodEndpoint));
    return app;
  }
}
