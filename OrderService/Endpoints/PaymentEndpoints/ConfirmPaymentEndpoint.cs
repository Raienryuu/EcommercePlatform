using System.Net;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class ConfirmPaymentEndpoint
{
  public static WebApplication MapConfirmPaymentEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.CONFIRM_PAYMENT,
        static async (
          IStripePaymentService stripePaymentService,
          HttpContext httpContext,
          CancellationToken ct
        ) =>
        {
          return await stripePaymentService.HandleWebhookPaymentConfirm(httpContext.Request);
        }
      )
      .WithName(nameof(ConfirmPaymentEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
