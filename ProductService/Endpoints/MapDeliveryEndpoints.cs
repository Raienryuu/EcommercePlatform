using ProductService.Endpoints.DeliveryEndpoints;

namespace ProductService.Endpoints;

public static class OrderEndpointMapper
{
  public static WebApplication MapDeliveryEndpoints(this WebApplication app)
  {
    app.MapGetAvailableDeliveries();

    return app;
  }
}
