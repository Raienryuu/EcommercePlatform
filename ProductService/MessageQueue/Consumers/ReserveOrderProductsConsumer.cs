using MassTransit;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using System.Diagnostics;

namespace ProductService.MessageQueue.Consumers
{
  public class ReserveOrderProductsConsumer : IConsumer<ReserveOrderProductsCommand>
  {
	private readonly ProductDbContext _db;
	private readonly ILogger _log;

	public ReserveOrderProductsConsumer(ProductDbContext db, ILogger<ReserveOrderProductsConsumer> log)
	{
	  _db = db;
	  _log = log;
	}

	public async Task Consume(ConsumeContext<ReserveOrderProductsCommand> context)
	{
	  var potentialReservation = await _db.OrdersReserved.FindAsync(context.Message.OrderId);

	  var isInDb = potentialReservation is not null;
	  if (isInDb)
	  {
		_log.LogWarning("Trying to reserve order {OrderId} that already has been reserved", context.Message.OrderId);
		return;
	  }

	  _log.LogInformation("Reserving products for order:{orderId}", context.Message.OrderId);

	  var products = context.Message.Products.ToList();

	  var storageProducts = await _db.Products.Where(x => products.Select(x => x.ProductId).ToList().Contains(x.Id)).ToListAsync();

	  if (storageProducts.Count != products.Count) { await PublishProductsNotAvaiable(context); return; }

	  storageProducts.ForEach(x => x.Quantity -= products.First(p => p.ProductId == x.Id).Quantity);
	  if (storageProducts.Any(x => x.Quantity < 0)) { await PublishProductsNotAvaiable(context); return; }

	  potentialReservation = new OrderReserved
	  {
		OrderId = context.Message.OrderId,
		IsReserved = true,
		ReserveTimestamp = DateTime.UtcNow
	  };

	  if (isInDb)
		_db.OrdersReserved.Update(potentialReservation);
	  else
		await _db.OrdersReserved.AddAsync(potentialReservation);

	  try
	  {
		await _db.SaveChangesAsync();
		_log.LogInformation("Products for order: {orderId} has been reserved succesfully.", context.Message.OrderId);
	  }
	  catch (DbUpdateException)
	  {
		throw;
	  }

	  await context.Publish<IOrderReserved>(new
	  {
		context.Message.OrderId,
	  }, context => context.CorrelationId = context.Message.OrderId);

	  return;
	}

	private async Task PublishProductsNotAvaiable(ConsumeContext<ReserveOrderProductsCommand> c)
	{
	  _log.LogInformation("Unable to reserve products for order: {orderId}", c.Message.OrderId);
	  await c.Publish<IOrderProductsNotAvailable>(new
	  {
		c.Message.OrderId,
	  }, context => context.CorrelationId = context.Message.OrderId);
	}
  }
}
