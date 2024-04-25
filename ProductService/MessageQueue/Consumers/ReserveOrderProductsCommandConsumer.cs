using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ProductService.MessageQueue.Consumers
{
  public class ReserveOrderProductsCommandConsumer : IConsumer<ReserveOrderProductsCommand>
  {
	private readonly ProductDbContext _db;
	private readonly ILogger _log;
	public ReserveOrderProductsCommandConsumer(ProductDbContext db, ILogger<ReserveOrderProductsCommandConsumer> log)
	{
	  _db = db;
	  _log = log;
	}
	public async Task Consume(ConsumeContext<ReserveOrderProductsCommand> context)
	{
	  _log.LogInformation("Reserving products for order:{orderId}", context.Message.OrderId);

	  var products = context.Message.Products.ToList();

	  var storageProducts = await _db.Products.Where(x => products.Select(x => x.ProductId).ToList().Contains(x.Id)).ToListAsync();

	  if (storageProducts.Count != products.Count) { await PublishProductsNotAvaiable(context); return; }

	  storageProducts.ForEach(x => x.Quantity -= products.First(p => p.ProductId == x.Id).Quantity);

	  if (storageProducts.Any(x => x.Quantity < 0))
	  {
		await PublishProductsNotAvaiable(context);
		return;
	  }
	  try
	  {
		await _db.SaveChangesAsync();
		_log.LogInformation("Products for order:{orderId} has been reserved succesfully.", context.Message.OrderId);
	  }
	  catch (Exception ex)
	  {
		throw new Exception("Unable to reserve products for order.", ex);
	  }

	  await context.Publish<IOrderReserved>(new
	  {
		context.Message.OrderId,
	  });

	  return;
	}

	private async Task PublishProductsNotAvaiable(ConsumeContext<ReserveOrderProductsCommand> c)
	{
	  _log.LogInformation("Unable to reserve products for order:{orderId}", c.Message.OrderId);
	  await c.Publish<IOrderProductsNotAvaiable>(new
	  {
		c.Message.OrderId,
	  });
	}
  }
}
