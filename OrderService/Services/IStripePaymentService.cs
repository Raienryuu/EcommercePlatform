using Common;
using OrderService.Models;
using Stripe;

namespace OrderService.Services;

public interface IStripePaymentService
{
  Task<ServiceResult<PaymentIntent>> CreatePaymentIntent(Order order, CancellationToken ct = default);
  Task<ServiceResult<PaymentIntent>> GetPaymentIntentForOrder(Order order, CancellationToken ct = default);
  Task<ServiceResult> HandleWebhookPaymentConfirm(HttpRequest request, CancellationToken ct = default);
  Task RefundPaymentForOrder(Guid orderId, CancellationToken ct = default);
}
