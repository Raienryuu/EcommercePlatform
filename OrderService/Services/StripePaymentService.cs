using System.Data;
using MassTransit;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Logging;
using OrderService.Models;
using OrderService.Options;
using Stripe;

namespace OrderService.Services;

public class StripePaymentService : IStripePaymentService
{
  private readonly PaymentIntentService _paymentService;
  private readonly IOptions<StripeConfig> _options;
  private readonly OrderDbContext _context;
  private readonly IPublishEndpoint _publisher;
  private readonly ILogger<StripePaymentService> _logger;

  public StripePaymentService(
    IOptions<StripeConfig> options,
    OrderDbContext context,
    IPublishEndpoint publisher,
    ILogger<StripePaymentService> logger
  )
  {
    var client = new StripeClient(options.Value.ApiKey);
    _paymentService = new PaymentIntentService(client);
    _options = options;
    _context = context;
    _publisher = publisher;
    _logger = logger;
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
      throw new ArgumentNullException(nameof(order), "Order does not have stripe payment id");
    }

    return _paymentService.GetAsync(order.StripePaymentId);
  }

  public async Task<IResult> HandleWebhookPaymentConfirm(HttpRequest request, CancellationToken ct = default)
  {
    var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
    var endpointSecret = _options.Value.Webhooks.PaymentIntentConfirmCode;
    var stripeEvent = EventUtility.ParseEvent(json);
    var signatureHeader = request.Headers["Stripe-Signature"];

    stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

    if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
    {
      throw new NullReferenceException("PaymentIntent object is null");
    }

    _logger.OrderSuccessfulPaymentInfo(paymentIntent.Amount);

    var order = await _context.Orders.FirstOrDefaultAsync(
      x => x.StripePaymentId == paymentIntent.Id,
      cancellationToken: ct
    );

    if (order is null)
    {
      return Results.NotFound("Order with given id doesn't exists.");
    }

    var isDuplicatedRequest = order.PaymentSucceded;

    if (!isDuplicatedRequest)
    {
      order.PaymentSucceded = true;
      await _context.SaveChangesAsync(ct);

      var productsIdsToReserve = order.Products.Select(p => new OrderProductDTO
      {
        ProductId = p.ProductId,
        Price = p.Price,
        Quantity = p.Quantity,
      });

      await _publisher.Publish<ReserveOrderProductsCommand>(
        new { order.OrderId, Products = productsIdsToReserve },
        ct
      );
      _logger.OrderReserveOrderCommandSent(order.OrderId);
    }
    return Results.Ok();
  }

  public async Task RefundPaymentForOrder(Guid orderId, CancellationToken ct = default)
  {
    var order = await _context.Orders.FindAsync([orderId], ct);

    if (order is null)
    {
      _logger.RefundingNonexistentOrder(orderId);
      return;
    }
    if (order?.StripePaymentId is null)
    {
      _logger.RefundingWithoutPaymentId(orderId);
      return;
    }

    if (!order.PaymentSucceded)
    {
      await CancelPaymentIntent(orderId, order.StripePaymentId);
      return;
    }

    var refundService = new RefundService();
    var refundOptions = new RefundCreateOptions { PaymentIntent = order?.StripePaymentId };

    var response = await refundService.CreateAsync(refundOptions, cancellationToken: ct);
    if (response.Status == "succeeded")
    {
      await _publisher.Publish<IOrderCancelledPaymentRefunded>(
        new
        {
          order!.OrderId,
          AmountInSmallestCurrencyUnit = response.Amount,
          CurrencyISO = response.Currency,
        },
        ct
      );
      _logger.OrderRefunded(order.OrderId, (int)response.Amount, response.Currency);
    }
  }

  private async Task CancelPaymentIntent(Guid orderId, string stripePaymentId)
  {
    try
    {
      var result = await _paymentService.CancelAsync(stripePaymentId);
      _logger.CancelledPaymentIntent(orderId);
      await _publisher.Publish<IOrderCancelledPaymentIntentCancelled>(new { OrderId = orderId });
    }
    catch (StripeException e)
    {
      _logger.UnableToCancelPaymentIntent(orderId, e.Message);
    }
  }
}
