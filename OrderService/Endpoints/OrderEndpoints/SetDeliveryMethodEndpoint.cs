using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Endpoints.OrderEndpoints;

public static class SetDeliveryMethodEndpoint
{
  public static WebApplication MapSetDeliveryMethodEndpoint(this WebApplication app)
  {
    _ = app.MapPatch(
        EndpointRoutes.Orders.SET_DELIVERY,
        async (
          OrderDbContext context,
          IPublishEndpoint publisher,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId,
          OrderDelivery deliveryMethod,
          CancellationToken ct
        ) =>
        {
          var order = await context.Orders.FindAsync([orderId], ct);

          if (order is null)
          {
            return Results.NotFound("No order found with given id.");
          }

          if (userId != order.UserId)
          {
            return Results.BadRequest("Mismatch between logged user Id and order's user Id.");
          }

          if (order.Delivery is not null)
          {
            return Results.BadRequest("Delivery method is already set.");
          }

          order.Delivery = deliveryMethod;

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

          return Results.NoContent();
        }
      )
      .WithName(nameof(SetDeliveryMethodEndpoint));
    return app;
  }
}
