using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using ProductService.MessageQueue.Consumers;
using ProductService.Tests.Fakes;

namespace ProductService.Tests;

public class ReserveProductOrderTests(DatabaseFixture db)
  : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _db = db;
  private static bool isStarted = false;
  private readonly object _lock = new();

  private void FillDatabase(ServiceProvider provider)
  {
	lock (_lock)
	{
	  if (isStarted)
		return;
	  using var scope = provider.CreateAsyncScope();
	  var dbContext
		= scope.ServiceProvider.GetRequiredService<ProductDbContext>();
	  dbContext.Database.EnsureCreated();
	  new ProductDbContextFakeBuilder().FillWithTestData(dbContext);
	  isStarted = true;
	}
  }

  [Fact]
  public async Task ReserveOrderProducts_ValidOrder_ProductsNotAvailable()
  {
	await using var provider = new ServiceCollection()
	  .AddDbContext<ProductDbContext, ProductDbContextFake>(o =>
		o.UseSqlServer(_db.GetConnectionString()))
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddConsumer<ReserveOrderProductsConsumer>();
		o.UsingInMemory((context, cfg) =>
		  cfg.ConfigureEndpoints(context));
	  })
	  .BuildServiceProvider(true);
	var harness = provider.GetRequiredService<ITestHarness>();
	await harness.Start();

	FillDatabase(provider);

	var orderId = Guid.NewGuid();
	await harness.Bus.Publish<ReserveOrderProductsCommand>(new()
	{
	  OrderId = orderId,
	  Products =
	  [
		new OrderProductDTO
		{
		  ProductId = 555,
		  Quantity = 1
		}
	  ]
	}, context => context.CorrelationId = orderId);

	var orderCommandConsumed
	  = await harness.Consumed.Any(x => x.Context.CorrelationId == orderId);
	var publishedResponse = harness.Published
	  .Select(x => x.Context.CorrelationId == orderId).ToList().Last();
	Assert.True(orderCommandConsumed);
	Assert.IsAssignableFrom<IOrderProductsNotAvailable>(publishedResponse
	  .MessageObject);
  }



  [Fact]
  public async Task ReserveOrderProducts_ValidOrder_ProductsReserved()
  {
	await using var provider = new ServiceCollection()
	  .AddDbContext<ProductDbContext, ProductDbContextFake>(o =>
		o.UseSqlServer(_db.GetConnectionString()))
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddConsumer<ReserveOrderProductsConsumer>();
		o.UsingInMemory((context, cfg) =>
		  cfg.ConfigureEndpoints(context));
	  })
	  .BuildServiceProvider(true);
	var harness = provider.GetRequiredService<ITestHarness>();
	await harness.Start();
	FillDatabase(provider);

	var orderId = Guid.NewGuid();
	await harness.Bus.Publish<ReserveOrderProductsCommand>(new()
	{
	  OrderId = orderId,
	  Products =
	  [
		new OrderProductDTO
		{
		  ProductId = 1,
		  Quantity = 2
		}
	  ]
	}, context => context.CorrelationId = orderId);
	await Task.Delay(1000);  // debug use only
	var orderCommandConsumed
	  = await harness.Consumed.Any(x => x.Context.CorrelationId == orderId);
	var publishedResponse = harness.Published
	  .Select(x => x.Context.CorrelationId == orderId).ToList().Last();
	Assert.True(orderCommandConsumed);
	Assert.IsAssignableFrom<IOrderReserved>(publishedResponse.MessageObject);

	using var scope = provider.CreateAsyncScope();
	var dbContext
	  = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
	var reservationRecord = await dbContext.OrdersReserved.FindAsync(orderId);
	Assert.NotNull(reservationRecord);
  }
}