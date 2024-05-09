using MassTransit;
using MassTransit.Testing;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;
namespace OrderService.Tests;

public class OrderServiceControllerUnitTests
{
  [Fact]
  public async Task PlaceOrder_Order_Ok201()
  {
	var order = new Order()
	{
	  OrderId = Guid.NewGuid(),
	  UserId = 1,
	  Products = [new OrderProduct { ProductId = 5, Quantity = 1 }]
	};
	await using var provider = new ServiceCollection()
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
		o.AddSagaStateMachineContainerTestHarness<NewOrderSaga, OrderState>();

		o.UsingInMemory((context, cfg) =>
		  cfg.ConfigureEndpoints(context));
	  })
	  .BuildServiceProvider(true);


	var harness = provider.GetRequiredService<ITestHarness>();

	await harness.Start();
	Assert.True(1 == 0);
  }
}