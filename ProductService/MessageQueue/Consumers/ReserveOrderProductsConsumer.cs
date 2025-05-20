using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.MessageQueue.Consumers;

public class ReserveOrderProductsConsumer(ProductDbContext db, ILogger<ReserveOrderProductsConsumer> log)
  : IConsumer<ReserveOrderProductsCommand>
{
  private readonly ILogger _log = log;

  public async Task Consume(ConsumeContext<ReserveOrderProductsCommand> context)
  {
    var potentialReservation = await db.OrdersReserved.FindAsync(context.Message.OrderId);

    var isInDb = potentialReservation is not null;
    if (isInDb)
    {
      _log.LogWarning(
        "Trying to reserve order {OrderId} that has been reserved already",
        context.Message.OrderId
      );
      return;
    }

    _log.LogInformation("Reserving products for order:{OrderId}", context.Message.OrderId);

    var products = context.Message.Products.ToList();

    var storageProducts = await db
      .Products.Where(x => products.Select(x => x.ProductId).ToList().Contains(x.Id))
      .ToListAsync();

    storageProducts.ForEach(x => x.Quantity -= products.First(p => p.ProductId == x.Id).Quantity);

    if (storageProducts.Count != products.Count || storageProducts.Any(x => x.Quantity < 0))
    {
      await PublishProductsNotAvaiable(context);
      return;
    }

    potentialReservation = new OrderReserved
    {
      OrderId = context.Message.OrderId,
      IsReserved = true,
      ReserveTimestamp = DateTime.UtcNow,
    };

    await db.OrdersReserved.AddAsync(potentialReservation);

    try
    {
      await db.SaveChangesAsync();
      _log.LogInformation(
        "Products for order: {orderId} has been reserved succesfully.",
        context.Message.OrderId
      );
    }
    catch (DbUpdateException)
    {
      _log.LogWarning("Unable to save changes for order: {orderId}", context.Message.OrderId);
      throw;
    }

    await context.Publish<IOrderReserved>(
      new { context.Message.OrderId },
      context => context.CorrelationId = context.Message.OrderId
    );

    return;
  }

  private async Task PublishProductsNotAvaiable(ConsumeContext<ReserveOrderProductsCommand> c)
  {
    _log.LogInformation("Unable to reserve products for order: {orderId}", c.Message.OrderId);
    await c.Publish<IOrderProductsNotAvailable>(
      new { c.Message.OrderId },
      context => context.CorrelationId = context.Message.OrderId
    );
  }
}
