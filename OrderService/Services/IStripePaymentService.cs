using OrderService.Models;
using Stripe;

namespace OrderService.Services;

public interface IStripePaymentService
{
  public Task<PaymentIntent> CreatePaymentIntent(Order order);
  public Task<PaymentIntent> GetPaymentIntentForOrder(Order order);
  Task<IResult> HandleWebhookPaymentConfirm(HttpRequest request, CancellationToken ct = default);
}
