using Common;
using OrderService.Models;
using OrderService.Services;
using Stripe;

namespace OrderService.Tests.Fakes;

public class FakeStripePaymentService() : IStripePaymentService
{
  public Task<ServiceResult<PaymentIntent>> CreatePaymentIntent(Order order, CancellationToken ct = default)
  {
    if (order.StripePaymentId is not null)
    {
      return GetPaymentIntentForOrder(order, ct);
    }

    var paymentIntent = new PaymentIntent() { ClientSecret = "secret" };
    var result = ServiceResults.Success(paymentIntent, System.Net.HttpStatusCode.OK);
    return Task.FromResult<ServiceResult<PaymentIntent>>(result);
  }

  public Task<ServiceResult<PaymentIntent>> GetPaymentIntentForOrder(Order order, CancellationToken ct = default)
  {
    var paymentIntent = new PaymentIntent() { ClientSecret = "oldSecret" };
    var result = ServiceResults.Success(paymentIntent, System.Net.HttpStatusCode.OK);
    return Task.FromResult<ServiceResult<PaymentIntent>>(result);
  }

  ///<remarks>
  ///  Only for Stripe webhook handler to use
  ///  </remarks>
  public Task<ServiceResult> HandleWebhookPaymentConfirm(HttpRequest request, CancellationToken ct = default)
  {
    var result = ServiceResults.Success(System.Net.HttpStatusCode.OK);
    return Task.FromResult<ServiceResult>(result);
  }

  public Task RefundPaymentForOrder(Guid orderId, CancellationToken ct = default)
  {
    return Task.CompletedTask;
  }
}
