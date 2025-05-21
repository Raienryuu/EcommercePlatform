using MassTransit;
using MessageQueue.Contracts;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Endpoints.OrderEndpoints;

public static class CancelOrderEndpoint
{
  public static WebApplication MapCancelOrderEndpoint(this WebApplication app)
  {
    app.MapDelete(
        EndpointRoutes.Orders.CANCEL_ORDER,
        async (
          OrderDbContext context,
          IPublishEndpoint publisher,
          CancellationToken ct,
          [FromHeader(Name = "UserId")] Guid userId,
          [FromRoute] Guid orderId
        ) =>
        {
          var order = await context.Orders.FindAsync(orderId);

          if (order is null)
          {
            return Results.NotFound();
          }
          if (order.UserId != userId)
          {
            return Results.BadRequest("Mismatch between logged user Id and order's user Id.");
          }

          if (!(order.Status is OrderStatus.Type.AwaitingConfirmation or OrderStatus.Type.Confirmed))
          {
            return Results.BadRequest("Too late to cancel the order.");
          }

          if (order.Status == OrderStatus.Type.Cancelled)
          {
            return Results.NoContent();
          }

          order.Status = OrderStatus.Type.Cancelled;
          context.Orders.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

          await context.SaveChangesAsync();
          await publisher.Publish<IOrderCancellationRequest>(new { order.OrderId });

          return Results.NoContent();
        }
      )
      .WithName(nameof(CancelOrderEndpoint));

    return app;
  }
}
