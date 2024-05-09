using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;
namespace OrderService.Tests;

public class NewOrderSagaTests
{
  [Fact]
  public async Task SagaInitiation_OnOrderSubmittedEvent_NewSagaInstanceCreated()
  {
	await using var provider = new ServiceCollection()
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
		//o.AddSagaStateMachineContainerTestHarness<NewOrderSaga, OrderState>();

		o.UsingInMemory((context, cfg) =>
		  cfg.ConfigureEndpoints(context));
	  })
	  .BuildServiceProvider(true);
	var harness = provider.GetRequiredService<ITestHarness>();
	await harness.Start();
	var sagasHarness = harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
	var orderId = Guid.NewGuid();

	await harness.Bus.Publish<IOrderSubmitted>(new
	{
	  OrderId = orderId,
	  Products = new OrderProductDTO[]
	  {
		new() { ProductId = 5, Quantity = 1 },
		new() { ProductId = 12, Quantity = 2 }
	  }
	});

	var doesInstanceExists = await sagasHarness.Sagas
	  .Any(s => s.CorrelationId == orderId);

	Assert.True(doesInstanceExists);
  }
}


