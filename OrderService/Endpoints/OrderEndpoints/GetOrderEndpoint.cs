using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetOrderEndpoint
{
  public static WebApplication MapGetOrderEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Orders.GET_ORDER,
        async Task<Results<Ok<Order>, ProblemHttpResult>> (
          IOrderService orderService,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          if (userId == Guid.Empty)
          {
            return TypedResults.Problem("User Id is required", statusCode: 400);
          }

          var results = await orderService.GetOrder(orderId, userId, ct);

          return results.IsSuccess
            ? TypedResults.Ok(results.Value)
            : TypedResults.Problem(results.ErrorMessage, statusCode: (int?)results.StatusCode);
        }
      )
      .WithName(nameof(GetOrderEndpoint));

    return app;
  }
}
