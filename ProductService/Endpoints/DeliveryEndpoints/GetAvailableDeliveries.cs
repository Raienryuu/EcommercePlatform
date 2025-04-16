using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Endpoints.DeliveryEndpoints;

public static class GetAvailableDeliveriesEndpoint
{
  public static WebApplication MapGetAvailableDeliveries(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Deliveries.GET_AVAILABLE_DELIVERIES,
        static async (ProductDbContext context, CancellationToken ct) =>
        {
          var deliveries = await context.Deliveries.ToListAsync(cancellationToken: ct);
          return Results.Ok(deliveries);
        }
      )
      .WithName(nameof(GetAvailableDeliveriesEndpoint))
      .Produces<List<Delivery>>((int)HttpStatusCode.OK);

    return app;
  }
}
