using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;

namespace OrderService.Tests;
public class NewOrderSagaTests
{
  [Fact]
  public async Task NewOrderSaga_SagaInitiationOnOrderSubmittedEvent_NewSagaInstanceCreated()
  {
	await using var provider = new ServiceCollection()
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
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

  [Fact]
  public async Task NewOrderSaga_OrderReserved_ConfirmedStatus()
  {
	await using var provider = new ServiceCollection()
	  .AddMassTransitTestHarness(o =>
	  {
		o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
		o.UsingInMemory((context, cfg) =>
		  cfg.ConfigureEndpoints(context));
	  })
	  .BuildServiceProvider(true);
	var harness = provider.GetRequiredService<ITestHarness>();
	await harness.Start();
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
	var sagasHarness = harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
	await sagasHarness.Exists(orderId);


	await harness.Bus.Publish<IOrderReserved>(new
	{
	  OrderId = orderId
	});

	
	var instanceConfirmed = await sagasHarness.Exists(orderId, 
	  (x => x.Confirmed));
	Assert.True(instanceConfirmed! == orderId);
  }
}


