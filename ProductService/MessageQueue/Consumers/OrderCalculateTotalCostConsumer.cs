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

  public async Task Consume(ConsumeContext<OrderCalculateTotalCostCommand> context)
  {
    var products = context.Message.Products.Select(p => p.ProductId);
    var productsFromDb = await db
      .Products.Where(x => products.Contains(x.Id))
      .ToListAsync(context.CancellationToken);
    if (productsFromDb.Count != products.Count())
    {
      _log.LogError("Couldn't find needed products for order: {orderId}", context.Message.OrderId);
      return;
    }
    var totalPrice = 0;
    productsFromDb.ForEach(p =>
    {
      totalPrice += (int)(p.Price * 100) * context.Message.Products.First(x => x.ProductId == p.Id).Quantity;
    });

    _log.LogInformation(
      "Calculated price for order: {orderId}, price: {price}",
      context.Message.OrderId,
      totalPrice
    );
    await context.Publish<IOrderPriceCalculated>(
      new
      {
        context.Message.OrderId,
        TotalPriceInSmallestCurrencyUnit = totalPrice,
        context.Message.CurrencyISO,
      }
    );

    // also add delivery price
  }
}
