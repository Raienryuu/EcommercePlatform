using Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Endpoints.PaymentEndpoints;

public static class GetPaymentStatusEndpoint
{
  public static WebApplication MapGetPaymentStatusEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.Payments.GET_PAYMENT_STATUS,
        static async Task<Results<Ok<PaymentStatus>, ProblemHttpResult>> (
          [FromHeader(Name = "UserId")] Guid userId,
          Guid orderId,
          OrderDbContext context,
          IOrderService orderService,
          IStripePaymentService paymentService,
          CancellationToken ct
        ) =>
        {
          var order = await orderService.GetOrder(orderId, userId, ct);

          if (!order.IsSuccess)
          {
            return TypedResults.Problem(order.ErrorMessage, statusCode: (int?)order.StatusCode);
          }

          if (order.Value.PaymentStatus is PaymentStatus.Succeded or PaymentStatus.Cancelled)
          {
            return TypedResults.Ok(order.Value.PaymentStatus);
          }

          var paymentIntent = await paymentService.GetPaymentIntentForOrder(order.Value, ct);

          var status = paymentIntent.Value.Status switch
          {
            "succeded" => PaymentStatus.Succeded,
            "pending" => PaymentStatus.Pending,
            "requires_payment_method" => PaymentStatus.Failed,
            _ => PaymentStatus.Pending,
          };

          return TypedResults.Ok(status);
        }
      )
      .WithName(nameof(GetPaymentStatusEndpoint));

    return app;
  }
}
