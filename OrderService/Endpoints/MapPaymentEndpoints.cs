using OrderService.Endpoints.PaymentEndpoints;

namespace OrderService.Endpoints;

public static class PaymentEndpointMapper
{
  public static WebApplication MapPaymentEndpoints(this WebApplication app)
  {
    app.MapCreateNewPaymentSessionEndpoint();
    app.MapGetPaymentStatusEndpoint();
    app.MapConfirmPaymentEndpoint();

    return app;
  }
}
