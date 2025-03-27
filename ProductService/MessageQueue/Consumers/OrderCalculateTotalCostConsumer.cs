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
    var totalPrice = 0m;
    productsFromDb.ForEach(p =>
    {
      totalPrice += p.Price * context.Message.Products.First(x => x.ProductId == p.Id).Quantity;
    });

    var normalizedPrice = (int)(totalPrice * 100);
    _log.LogInformation(
      "Calculated price for order: {orderId}, price: {price}",
      context.Message.OrderId,
      normalizedPrice
    );
    await context.Publish<IOrderPriceCalculated>(
      new
      {
        context.Message.OrderId,
        TotalPriceInSmallestCurrencyUnit = normalizedPrice,
        context.Message.CurrencyISO,
      }
    );

    // also add delivery price
  }
}
