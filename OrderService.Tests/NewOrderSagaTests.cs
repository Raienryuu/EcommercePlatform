using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;
using OrderService.Tests.Fakes;

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
			(x => x.Confirmed), TimeSpan.FromSeconds(1));
		Assert.True(instanceConfirmed! == orderId);
	}

	[Fact]
	public async Task NewOrderSaga_OrderProductsNotAvaiable_CancelledStatus()
	{
		await using var provider = new ServiceCollection()
			.AddSingleton<OrderDbContext, FakeOrderDbContext>()
			.AddSingleton<DbContextOptions>(x => new DbContextOptionsBuilder<FakeOrderDbContext>().UseInMemoryDatabase($"orderSagas-{Guid.NewGuid()}").Options)
			.AddMassTransitTestHarness(o =>
			{
				o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
				o.UsingInMemory((context, cfg) =>
				cfg.ConfigureEndpoints(context));
			})
			.BuildServiceProvider(true);
		var orderId = Guid.NewGuid();
		var db = provider.GetRequiredService<OrderDbContext>();
		db.Orders.Add(new Order
		{
			OrderId = orderId,
		});
		db.SaveChanges();
		var harness = provider.GetRequiredService<ITestHarness>();
		await harness.Start();
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
		var pendingSaga = await sagasHarness.Exists(orderId, (x => x.Pending));


		await harness.Bus.Publish<IOrderProductsNotAvaiable>(new
		{
			OrderId = orderId
		});


		var instanceConfirmed = await sagasHarness.Exists(orderId,
			(x => x.Cancelled), TimeSpan.FromSeconds(1));
		Assert.True(instanceConfirmed! == orderId);
	}
}


