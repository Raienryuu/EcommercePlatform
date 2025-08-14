using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class ConfirmPaymentEndpoint
{
  public static WebApplication MapConfirmPaymentEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.CONFIRM_PAYMENT,
        static async Task<Results<Ok, ProblemHttpResult>> (
          IStripePaymentService stripePaymentService,
          HttpContext httpContext,
          CancellationToken ct
        ) =>
        {
          var result = await stripePaymentService.HandleWebhookPaymentConfirm(httpContext.Request, ct);
          return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.Problem(result.ErrorMessage, statusCode: (int?)result.StatusCode);
        }
      )
      .WithName(nameof(ConfirmPaymentEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
