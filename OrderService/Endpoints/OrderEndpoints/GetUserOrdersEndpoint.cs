using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Endpoints.OrderEndpoints;

public static class GetUserOrdersEndpoint
{
  public static WebApplication MapGetUserOrdersEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Orders.GET_USER_ORDERS,
        async (OrderDbContext context, CancellationToken ct, [FromHeader(Name = "UserId")] Guid userId) =>
        {
          var orders = await context.Orders.Where(x => x.UserId == userId).ToListAsync();

          return Results.Ok(orders);
        }
      )
      .WithName(nameof(GetUserOrdersEndpoint));

    return app;
  }
}
