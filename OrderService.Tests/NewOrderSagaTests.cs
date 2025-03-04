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
        _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
        o.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .BuildServiceProvider(true);
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    var sagasHarness = harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
    var orderId = Guid.NewGuid();

    await harness.Bus.Publish<IOrderSubmitted>(
      new
      {
        OrderId = orderId,
        Products = new OrderProductDTO[]
        {
          new() { ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"), Quantity = 1 },
          new() { ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"), Quantity = 2 },
        },
      }
    );

    var doesInstanceExists = await sagasHarness.Sagas.Any(s => s.CorrelationId == orderId);
    Assert.True(doesInstanceExists);
  }

  [Fact]
  public async Task NewOrderSaga_OrderReserved_ConfirmedStatus()
  {
    await using var provider = new ServiceCollection()
      .AddMassTransitTestHarness(static o =>
      {
        _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
        o.UsingInMemory(static (context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .BuildServiceProvider(true);
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    var orderId = Guid.NewGuid();
    await harness.Bus.Publish<IOrderSubmitted>(
      new
      {
        OrderId = orderId,
        Products = new OrderProductDTO[]
        {
          new() { ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"), Quantity = 1 },
          new() { ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"), Quantity = 2 },
        },
      }
    );
    var sagasHarness = harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
    _ = await sagasHarness.Exists(orderId);

    await harness.Bus.Publish<IOrderReserved>(new { OrderId = orderId });

    var instanceConfirmed = await sagasHarness.Exists(
      orderId,
      static x => x.Confirmed,
      TimeSpan.FromSeconds(1)
    );
    Assert.True(instanceConfirmed! == orderId);
  }

  [Fact]
  public async Task NewOrderSaga_OrderProductsNotAvaiable_CancelledStatus()
  {
    await using var provider = new ServiceCollection()
      .AddSingleton<OrderDbContext, FakeOrderDbContext>()
      .AddSingleton<DbContextOptions>(static x =>
        new DbContextOptionsBuilder<FakeOrderDbContext>()
          .UseInMemoryDatabase($"orderSagas-{Guid.NewGuid()}")
          .Options
      )
      .AddMassTransitTestHarness(static o =>
      {
        _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
        o.UsingInMemory(static (context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .BuildServiceProvider(true);
    var orderId = Guid.NewGuid();
    var db = provider.GetRequiredService<OrderDbContext>();
    _ = db.Orders.Add(new Order { OrderId = orderId });
    _ = db.SaveChanges();
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    await harness.Bus.Publish<IOrderSubmitted>(
      new
      {
        OrderId = orderId,
        Products = new OrderProductDTO[]
        {
          new() { ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"), Quantity = 1 },
          new() { ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"), Quantity = 2 },
        },
      }
    );
    var sagasHarness = harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
    var pendingSaga = await sagasHarness.Exists(orderId, static x => x.Pending);

    await harness.Bus.Publish<IOrderProductsNotAvailable>(new { OrderId = orderId });

    var instanceConfirmed = await sagasHarness.Exists(
      orderId,
      static x => x.Cancelled,
      TimeSpan.FromSeconds(1)
    );
    Assert.True(instanceConfirmed! == orderId);
  }
}
