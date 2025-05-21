using System.Net;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
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
        static async (
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
                .Orders.AsNoTracking()
                .Where(x => x.StripePaymentId == eventData.PaymentIntentId)
                .FirstOrDefaultAsync(cancellationToken: ct) ?? throw new Exception("got him!");

            if (eventData.Status == "succeeded")
            {
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
            Console.WriteLine($"Stripe error: {e.Message}");
            return Results.BadRequest();
          }

          return Results.Ok();
        }
      )
      .WithName(nameof(RefundUpdateWebhookEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
