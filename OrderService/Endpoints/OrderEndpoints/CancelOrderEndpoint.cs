using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CancelOrderEndpoint
{
  public static WebApplication MapCancelOrderEndpoint(this WebApplication app)
  {
    app.MapDelete(
        EndpointRoutes.Orders.CANCEL_ORDER,
        async Task<Results<NoContent, ProblemHttpResult>> (
          IOrderService orderService,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          if (orderId == Guid.Empty)
          {
            return TypedResults.Problem("Order Id is required", statusCode: 400);
          }

          var result = await orderService.CancelOrder(orderId, userId, ct);
          return result.IsSuccess
            ? TypedResults.NoContent()
            : TypedResults.Problem(result.ErrorMessage, statusCode: (int?)result.StatusCode);
        }
      )
      .WithName(nameof(CancelOrderEndpoint));

    return app;
  }
}
