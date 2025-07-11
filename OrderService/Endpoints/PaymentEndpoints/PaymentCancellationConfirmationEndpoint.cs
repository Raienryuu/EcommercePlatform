using System.Net;
using Contracts;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Logging;
using OrderService.Options;
using OrderService.Services;
using Stripe;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class RefundUpdateWebhookEndpoint
{
  public static WebApplication MapRefundUpdateWebhookEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.REFUND_UPDATE_WEBHOOK,
        static async Task<Results<Ok, BadRequest>> (
          IStripePaymentService stripePaymentService,
          OrderDbContext orderDb,
          IOptions<StripeConfig> options,
          IPublishEndpoint publisher,
          ILoggerFactory loggerFactory,
          HttpContext httpContext,
          CancellationToken ct
        ) =>
        {
          var logger = loggerFactory.CreateLogger(nameof(RefundUpdateWebhookEndpoint));
          var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync(ct);

          try
          {
            var endpointSecret = options.Value.Webhooks.RefundCode;
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = httpContext.Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

            if (stripeEvent.Data.Object is not Refund eventData)
            {
              throw new NullReferenceException("Refund object is null");
            }

            var order =
              await orderDb
                .Orders.Where(x => x.StripePaymentId == eventData.PaymentIntentId)
                .FirstOrDefaultAsync(cancellationToken: ct)
              ?? throw new Exception("Did not found relevant order");

            if (eventData.Status == "succeeded")
            {
              order.PaymentStatus = PaymentStatus.Cancelled;

              await orderDb.SaveChangesAsync(ct);
              await publisher.Publish<IOrderCancelledPaymentRefunded>(
                new
                {
                  order.OrderId,
                  AmountInSmallestCurrencyUnit = eventData.Amount,
                  CurrencyISO = eventData.Currency,
                },
                ct
              );
              logger.OrderRefunded(order.OrderId, (int)eventData.Amount, eventData.Currency);
            }
          }
          catch (StripeException e)
          {
            logger.LogError("Stripe error: {message}", e.Message);
            return TypedResults.BadRequest();
          }

          return TypedResults.Ok();
        }
      )
      .WithName(nameof(RefundUpdateWebhookEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
