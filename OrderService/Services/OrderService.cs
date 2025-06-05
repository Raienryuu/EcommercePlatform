using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Endpoints.Requests;
using OrderService.Logging;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService(ILogger<OrderService> logger, OrderDbContext context, IPublishEndpoint publisher)
  : IOrderService
{
  public async Task<Order> CreateOrder(
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

    return newOrder;
  }

  public async Task<(bool isSuccess, string error)> SetDeliveryMethod(
    Guid orderId,
    Guid userId,
    OrderDelivery deliveryMethod,
    CancellationToken ct
  )
  {
    var order = await context.Orders.FindAsync([orderId], ct);

    if (order is null)
    {
      return (false, "No order found with given id.");
    }

    if (userId != order.UserId)
    {
      return (false, "Mismatch between logged user Id and order's user Id.");
    }

    if (order.Delivery is not null)
    {
      return (false, "Delivery method is already set.");
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

    return (true, "");
  }

  public async Task<(bool isSuccess, string error)> CancelOrder(
    Guid orderId,
    Guid userId,
    CancellationToken ct
  )
  {
    var order = await context.Orders.FindAsync([orderId], cancellationToken: ct);

    if (order is null)
    {
      return (false, "Order not found");
    }
    if (order.UserId != userId)
    {
      return (false, "Mismatch between logged user Id and order's user Id.");
    }

    if (!(order.Status is OrderStatus.AwaitingConfirmation or OrderStatus.Confirmed))
    {
      return (false, "Too late to cancel the order.");
    }

    if (order.Status == OrderStatus.Cancelled)
    {
      return (true, "");
    }

    order.Status = OrderStatus.Cancelled;
    context.Orders.Entry(order).State = EntityState.Modified;

    await context.SaveChangesAsync(ct);
    await publisher.Publish<IOrderCancellationRequest>(new { order.OrderId }, ct);
    return (true, "");
  }

  public async Task<(Order?, string error)> GetOrder(Guid orderId, Guid userId, CancellationToken ct)
  {
    var order = await context.Orders.FindAsync([orderId], ct);
    return userId != order?.UserId ? (null, "Mismatch between logged user Id and order's user Id.")
      : order is null ? (null, "Order not found")
      : (order, "");
  }

  public Task<List<Order>> GetUserOrders(Guid userId, CancellationToken ct)
  {
    return context.Orders.Where(x => x.UserId == userId).ToListAsync(cancellationToken: ct);
  }
}
