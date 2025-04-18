using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class GetPaymentStatusEndpoint
{
  public static WebApplication MapGetPaymentStatusEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Payments.GET_PAYMENT_STATUS,
        static async (
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

          if (order.PaymentSucceded)
          {
            return Results.Ok("succeeded");
          }

          var paymentIntent = await paymentService.GetPaymentIntentForOrder(order);
          return Results.Ok(paymentIntent.Status);
        }
      )
      .WithName(nameof(GetPaymentStatusEndpoint))
      .Produces<string>((int)HttpStatusCode.OK);

    return app;
  }
}
