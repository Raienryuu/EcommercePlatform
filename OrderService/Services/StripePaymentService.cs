using Microsoft.Extensions.Options;
using OrderService.Models;
using OrderService.Options;
using Stripe;

namespace OrderService.Services;

public class StripePaymentService : IStripePaymentService
{
  private readonly PaymentIntentService _paymentService;

  public StripePaymentService(IOptions<StripeConfig> options)
  {
    var client = new StripeClient(options.Value.ApiKey);
    _paymentService = new PaymentIntentService(client);
  }

  public Task<PaymentIntent> CreatePaymentIntent(Order order)
  {
    var requestOptions = new RequestOptions() { IdempotencyKey = order.OrderId.ToString() };
    var paymentOptions = new PaymentIntentCreateOptions
    {
      Amount = order.TotalPriceInSmallestCurrencyUnit,
      Currency = order.CurrencyISO,
      AutomaticPaymentMethods = new() { Enabled = true },
      Description = "OrderId: " + order.OrderId.ToString(),
      UseStripeSdk = true,
    };

    return _paymentService.CreateAsync(paymentOptions, requestOptions);
  }

  public Task<PaymentIntent> GetPaymentIntentForOrder(Order order)
  {
    if (order.StripePaymentId is null)
    {
      throw new ArgumentNullException(nameof(order));
    }
    return _paymentService.GetAsync(order.StripePaymentId);
  }
}
