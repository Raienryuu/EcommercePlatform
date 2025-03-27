using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class CreateNewPaymentSessionEndpoint
{
  public static WebApplication MapCreateNewPaymentSessionEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.CREATE_NEW_PAYMENT_SESSION,
        async (
          [FromHeader(Name = "UserId")] Guid userId,
          Guid orderId,
          OrderDbContext context,
          IStripePaymentService paymentService,
          CancellationToken ct
        ) =>
        {
          var order = await context.Orders.FindAsync([orderId], cancellationToken: ct);

          if (order is null)
          {
            return Results.NotFound("Order with given id doesn't exists.");
          }

          if (order.UserId != userId)
          {
            return Results.BadRequest("Mismatch between logged user Id and order's user Id.");
          }

          if (order.TotalPriceInSmallestCurrencyUnit is null)
          {
            return Results.AcceptedAtRoute(
              nameof(CreateNewPaymentSessionEndpoint),
              new { orderId = order.OrderId }
            );
          }

          if (order.StripePaymentId is not null)
          {
            var oldIntent = await paymentService.GetPaymentIntentForOrder(order);
            return Results.Ok(oldIntent.ClientSecret);
          }

          var paymentIntent = await paymentService.CreatePaymentIntent(order);
          order.StripePaymentId = paymentIntent.Id;
          await context.SaveChangesAsync(ct);

          return Results.Ok(paymentIntent.ClientSecret);
        }
      )
      .WithName(nameof(CreateNewPaymentSessionEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
