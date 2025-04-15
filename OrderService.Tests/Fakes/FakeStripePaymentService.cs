using OrderService.Models;
using OrderService.Services;
using Stripe;

namespace OrderService.Tests.Fakes;

public class FakeStripePaymentService() : IStripePaymentService
{
  public Task<PaymentIntent> CreatePaymentIntent(Order order)
  {
    if (order.StripePaymentId is not null)
    {
      return GetPaymentIntentForOrder(order);
    }

    return Task.FromResult(new PaymentIntent() { ClientSecret = "secret" });
  }

  public Task<PaymentIntent> GetPaymentIntentForOrder(Order order)
  {
    return Task.FromResult(new PaymentIntent() { ClientSecret = "oldSecret" });
  }

  ///<remarks>
  ///  Only for Stripe webhook handler to use
  ///  </remarks>
  public Task<IResult> HandleWebhookPaymentConfirm(HttpRequest request, CancellationToken ct = default)
  {
    throw new NotImplementedException();
  }
}
