using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ProductService.MessageQueue.Consumers;

public class OrderCalculateTotalCostConsumer(
  ProductDbContext db,
  ILogger<OrderCalculateTotalCostConsumer> log
) : IConsumer<OrderCalculateTotalCostCommand>
{
  private readonly ILogger _log = log;

  public async Task Consume(ConsumeContext<OrderCalculateTotalCostCommand> command)
  {
    var products = command.Message.Products.Select(p => p.ProductId);
    var productsFromDb = await db
      .Products.Where(x => products.Contains(x.Id))
      .ToListAsync(command.CancellationToken);
    var delivery = await db.Deliveries.FindAsync(command.Message.DeliveryId);

    if (productsFromDb.Count != products.Count())
    {
      _log.CouldntFindProducts(command.Message.OrderId);
      return;
    }
    if (delivery is null)
    {
      _log.CouldntFindDeliveryItem(command.Message.DeliveryId);
      return;
    }

    var totalPrice = 0;
    totalPrice += (int)delivery.Price;
    productsFromDb.ForEach(p =>
    {
      totalPrice += (int)(p.Price * 100) * command.Message.Products.First(x => x.ProductId == p.Id).Quantity;
    });
    _log.OrderTotalCalculatedSuccessfully(command.Message.OrderId, totalPrice);

    await command.Publish<IOrderPriceCalculated>(
      new
      {
        command.Message.OrderId,
        TotalPriceInSmallestCurrencyUnit = totalPrice,
        command.Message.CurrencyISO,
      }
    );
  }
}
