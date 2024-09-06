using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductService.MessageQueue.Consumers;
using ProductService.Models;

namespace ProductService.Tests;

public class ReserveProductTemp : ReserveOrderProductsCommandConsumer
{
  public ReserveProductTemp(ProductDbContext db, ILogger<ReserveOrderProductsCommandConsumer> log) : base(db, log)
  {
  }
}

public class ReserveProductOrderCommandTests(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _db = db;

  [Fact]
  public async Task ReserveOrderProducts_ValidOrder_ProductsNotAvailable()
  {
	await using var provider = new ServiceCollection()
	  .AddSingleton<DbContextOptions>(new DbContextOptionsBuilder()
	  .UseSqlServer(connectionString: _db.GetConnectionString()).Options)
	  .AddSingleton<ProductDbContext>()
		.AddMassTransitTestHarness(o =>
		{
		  o.AddConsumer<ReserveProductTemp>();
		  o.UsingInMemory((context, cfg) =>
			  cfg.ConfigureEndpoints(context));
		})
		.BuildServiceProvider(true);

	await provider.GetRequiredService<ProductDbContext>().Database.EnsureCreatedAsync();
	var harness = provider.GetRequiredService<ITestHarness>();
	await harness.Start();

	var orderId = Guid.NewGuid();
	var client = harness.Bus.CreateRequestClient<ReserveOrderProductsCommand>();

	await harness.Bus.Publish<ReserveOrderProductsCommand>(new()
	{
	  OrderId = orderId,
	  Products = [new OrderProductDTO {
		ProductId = 555,
		Quantity = 1
	  }]
	},
	context => context.CorrelationId = orderId);

	//await Task.Delay(1000); 138 test passes without failures

	var orderCommandConsumed = await harness.Consumed.Any(x => x.Context.CorrelationId == orderId);
	var publishedResponse = harness.Published.Select(x => x.Context.CorrelationId == orderId).ToList().ElementAt(1);
	Assert.True(orderCommandConsumed);
	Assert.IsAssignableFrom<IOrderProductsNotAvaiable>(publishedResponse.MessageObject);
  }
}
