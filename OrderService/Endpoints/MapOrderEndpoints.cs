using OrderService.Endpoints.OrderEndpoints;

namespace OrderService.Endpoints;

public static class OrderEndpointMapper
{
  public static WebApplication MapOrderEndpoints(this WebApplication app)
  {
    app.MapGetOrderEndpoint();
    app.MapCreateOrderEndpoint();
    app.MapDeleteOrderEndpoint();
    app.MapPutOrderEndpoint();
    app.MapGetUserOrdersEndpoint();
    app.MapSetDeliveryMethodEndpoint();

    return app;
  }
}
