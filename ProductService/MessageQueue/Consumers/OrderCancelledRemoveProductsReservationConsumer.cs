using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ProductService.MessageQueue.Consumers;

public class OrderCancelledRemoveProductsReservationConsumer(ProductDbContext db, ILogger<OrderCancelledRemoveProductsReservationConsumer> log)
  : IConsumer<OrderCancelledRemoveProductsReservationCommand>
{
  public async Task Consume(ConsumeContext<OrderCancelledRemoveProductsReservationCommand> context)
  {
    var reservation = await db.OrdersReserved.Where(x => x.OrderId == context.Message.OrderId).FirstOrDefaultAsync();
    if (reservation is not null)
    {
      db.OrdersReserved.Remove(reservation);
      await db.SaveChangesAsync();
      log.ReservationRemoved(context.Message.OrderId);
    }
    else
    {
      log.CouldntFindOrderReservation(context.Message.OrderId);
    }

    await context.Publish<IOrderCancelledRemovedProductsReservation>(new { context.Message.OrderId });


  }
}

