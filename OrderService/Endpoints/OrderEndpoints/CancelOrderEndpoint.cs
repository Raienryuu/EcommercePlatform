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
          var result = await orderService.CancelOrder(orderId, userId, ct);
          return result.IsSuccess
            ? TypedResults.NoContent()
            : TypedResults.Problem(result.ErrorMessage, statusCode: result.StatusCode);
        }
      )
      .WithName(nameof(CancelOrderEndpoint));

    return app;
  }
}
