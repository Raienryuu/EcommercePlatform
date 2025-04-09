using System.Net;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class ConfirmPaymentEndpoint
{
  public static WebApplication MapConfirmPaymentEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.CONFIRM_PAYMENT,
        static async (OrderDbContext context, HttpContext httpContext, CancellationToken ct) =>
        {
          var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync(ct);
          const string ENDPOINT_SECRET = "whsec_KlrpbD4yBUkNSVB9cI8SLdmH47l2pL4z";
          var stripeEvent = EventUtility.ParseEvent(json);
          var signatureHeader = httpContext.Request.Headers["Stripe-Signature"];

          stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, ENDPOINT_SECRET);

          if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
          {
            throw new NullReferenceException("PaymentIntent object is null");
          }

          /*Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);*/

          var order = await context.Orders.FirstOrDefaultAsync(
            x => x.StripePaymentId == paymentIntent.Id,
            cancellationToken: ct
          );

          if (order is null)
          {
            return Results.NotFound("Order with given id doesn't exists.");
          }

          order.PaymentSucceded = true;
          await context.SaveChangesAsync(ct);

          return Results.Ok();
        }
      )
      .WithName(nameof(ConfirmPaymentEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
