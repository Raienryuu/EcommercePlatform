using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Endpoints.DeliveryEndpoints;

public static class GetAvailableDeliveriesEndpoint
{
  public static WebApplication MapGetAvailableDeliveries(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Deliveries.GET_AVAILABLE_DELIVERIES,
        static async Task<Results<Ok<List<Delivery>>, ProblemHttpResult>> (
          IDeliveryService deliveryService,
          CancellationToken ct
        ) =>
        {
          var deliveries = await deliveryService.GetAvailableDeliveries();
          if (deliveries.IsSuccess)
          {
            return TypedResults.Ok(deliveries.Value);
          }
          return TypedResults.Problem(deliveries.ErrorMessage, statusCode: deliveries.StatusCode);
        }
      )
      .WithName(nameof(GetAvailableDeliveriesEndpoint))
      .Produces<List<Delivery>>((int)HttpStatusCode.OK);

    return app;
  }
}
