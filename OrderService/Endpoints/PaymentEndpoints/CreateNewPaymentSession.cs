using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class CreateNewPaymentSessionEndpoint
{
  public static WebApplication MapCreateNewPaymentSessionEndpoint(this WebApplication app)
  {
    app.MapPost(
        EndpointRoutes.Payments.CREATE_NEW_PAYMENT_SESSION,
        async Task<Results<Ok<string>, ProblemHttpResult, AcceptedAtRoute>> (
          [FromHeader(Name = "UserId")] Guid userId,
          Guid orderId,
          OrderDbContext context,
          IStripePaymentService paymentService,
          CancellationToken ct
        ) =>
        {
          if (orderId == Guid.Empty)
          {
            return TypedResults.Problem("Order Id is required", statusCode: 400);
          }

          var order = await context.Orders.FindAsync([orderId], cancellationToken: ct);

          if (order is null)
          {
            return TypedResults.Problem("Order with given id doesn't exists.", statusCode: 404);
          }

          if (order.UserId != userId)
          {
            return TypedResults.Problem(
              "Mismatch between logged user Id and order's user Id.",
              statusCode: 402
            );
          }

          if (order.TotalPriceInSmallestCurrencyUnit is null)
          {
            return TypedResults.AcceptedAtRoute(
              nameof(CreateNewPaymentSessionEndpoint),
              new { orderId = order.OrderId }
            );
          }

          if (order.StripePaymentId is not null)
          {
            var oldIntent = await paymentService.GetPaymentIntentForOrder(order, ct);
            if (oldIntent.IsSuccess)
            {
              return TypedResults.Ok(oldIntent.Value.ClientSecret);
            }
          }

          var paymentIntent = await paymentService.CreatePaymentIntent(order, ct);
          if (!paymentIntent.IsSuccess)
          {
            return TypedResults.Problem(paymentIntent.ErrorMessage, statusCode: paymentIntent.StatusCode);
          }

          order.StripePaymentId = paymentIntent.Value.Id;
          await context.SaveChangesAsync(ct);

          return TypedResults.Ok(paymentIntent.Value.ClientSecret);
        }
      )
      .WithName(nameof(CreateNewPaymentSessionEndpoint));

    return app;
  }
}
