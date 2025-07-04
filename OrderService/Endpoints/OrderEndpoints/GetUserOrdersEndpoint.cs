using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetUserOrdersEndpoint
{
  public static WebApplication MapGetUserOrdersEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Orders.GET_USER_ORDERS,
        async Task<Ok<List<Order>>> (
          IOrderService orderService,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId
        ) =>
        {
          var orders = await orderService.GetUserOrders(userId, ct);
          return TypedResults.Ok(orders);
        }
      )
      .WithName(nameof(GetUserOrdersEndpoint));

    return app;
  }
}
