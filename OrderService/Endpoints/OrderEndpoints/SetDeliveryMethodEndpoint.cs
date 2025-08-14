using Microsoft.AspNetCore.Http.HttpResults;
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
        async Task<Results<NoContent, ProblemHttpResult>> (
          IOrderService orderService,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId,
          OrderDelivery deliveryMethod,
          CancellationToken ct
        ) =>
        {
          if (userId == Guid.Empty)
          {
            return TypedResults.Problem("User Id is required", statusCode: 400);
          }

          var result = await orderService.SetDeliveryMethod(orderId, userId, deliveryMethod, ct);

          return result.IsSuccess
            ? TypedResults.NoContent()
            : TypedResults.Problem(result.ErrorMessage, statusCode: (int?)result.StatusCode);
        }
      )
      .WithName(nameof(SetDeliveryMethodEndpoint));
    return app;
  }
}
