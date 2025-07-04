using Common;
using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using OrderService.Endpoints.Requests;
using OrderService.Logging;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService(ILogger<OrderService> logger, OrderDbContext context, IPublishEndpoint publisher)
  : IOrderService
{
  public async Task<ServiceResult<Order>> CreateOrder(
    Guid userId,
    CreateOrderRequest orderRequest,
    CancellationToken ct = default
  )
  {
    var newOrder = new Order
    {
      OrderId = NewId.NextGuid(),
      UserId = userId,
      IsConfirmed = false,
      Created = DateTime.UtcNow,
      LastModified = DateTime.UtcNow,
      Notes = orderRequest.Notes,
      Products = orderRequest.Products,
      StripePaymentId = null,
      CurrencyISO = orderRequest.CurrencyISO,
      Status = OrderStatus.AwaitingConfirmation,
    };

    _ = await context.Orders.AddAsync(newOrder, ct);
    _ = await context.SaveChangesAsync(ct);

    await publisher.Publish<IOrderCreatedByUser>(
      new
      {
        newOrder.OrderId,
        newOrder.Products,
        newOrder.CurrencyISO,
      },
      ct
    );

    logger.NewOrderCreated(newOrder.OrderId);

    return ServiceResults.Success(newOrder, 201);
  }

  public async Task<ServiceResult> SetDeliveryMethod(
    Guid orderId,
    Guid userId,
    OrderDelivery deliveryMethod,
    CancellationToken ct
  )
  {
    var order = await context.Orders.FindAsync([orderId], ct);

    if (order is null)
    {
      return ServiceResults.Error("No order found with given id.", 404);
    }

    if (userId != order.UserId)
    {
      return ServiceResults.Error("Mismatch between logged user Id and order's user Id.", 401);
    }

    if (order.Delivery is not null)
    {
      return ServiceResults.Error("Delivery method is already set.", 400);
    }

    order.Delivery = deliveryMethod;

    context.Orders.Entry(order).State = EntityState.Modified;

    _ = await context.SaveChangesAsync(ct);

    await publisher.Publish<OrderCalculateTotalCostCommand>(
      new
      {
        order.OrderId,
        order.Products,
        order.CurrencyISO,
        EurToCurrencyMultiplier = 1m,
        deliveryMethod.DeliveryId,
      },
      ct
    );

    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult> CancelOrder(Guid orderId, Guid userId, CancellationToken ct)
  {
    var order = await context.Orders.FindAsync([orderId], cancellationToken: ct);

    if (order is null)
    {
      return ServiceResults.Error("Order not found", 404);
    }
    if (order.UserId != userId)
    {
      return ServiceResults.Error("Mismatch between logged user Id and order's user Id.", 401);
    }

    if (!(order.Status is OrderStatus.AwaitingConfirmation or OrderStatus.Confirmed))
    {
      return ServiceResults.Error("Too late to cancel the order.", 400);
    }

    if (order.Status == OrderStatus.Cancelled)
    {
      return ServiceResults.Success(200);
    }

    order.Status = OrderStatus.Cancelled;
    context.Orders.Entry(order).State = EntityState.Modified;

    await context.SaveChangesAsync(ct);
    await publisher.Publish<IOrderCancellationRequest>(new { order.OrderId }, ct);
    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult<Order>> GetOrder(Guid orderId, Guid userId, CancellationToken ct)
  {
    var order = await context.Orders.FindAsync([orderId], ct);

    if (order is null)
    {
      return ServiceResults.Error<Order>("Order not found", 404);
    }

    if (userId != order.UserId)
    {
      return ServiceResults.Error<Order>("Mismatch between logged user Id and order's user Id.", 401);
    }

    return ServiceResults.Success(order, 200);
  }

  public Task<List<Order>> GetUserOrders(Guid userId, CancellationToken ct)
  {
    return context.Orders.Where(x => x.UserId == userId).ToListAsync(cancellationToken: ct);
  }
}
