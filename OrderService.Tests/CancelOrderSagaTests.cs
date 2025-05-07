using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;

namespace OrderService.Tests;

public class CancelOrderSagaTests
{
  private readonly ServiceProvider _provider;
  private readonly ITestHarness _harness;

  private readonly ISagaStateMachineTestHarness<CancelOrderSaga, CancelOrderState> _cancelSagasHarness;
  private readonly ISagaStateMachineTestHarness<NewOrderSaga, OrderState> _newSagasHarness;
  private readonly ILogger<CancelOrderSagaTests> _logger;

  public CancelOrderSagaTests()
  {
    _provider = SetupServiceProvider();
    _harness = _provider.GetRequiredService<ITestHarness>();
    _harness.Start().Wait();

    _logger = _provider.GetRequiredService<ILogger<CancelOrderSagaTests>>();
    _cancelSagasHarness = _harness.GetSagaStateMachineHarness<CancelOrderSaga, CancelOrderState>();
    _newSagasHarness = _harness.GetSagaStateMachineHarness<NewOrderSaga, OrderState>();
  }

  private static async Task PublishNewOrderWithId(Guid orderId, ITestHarness harness)
  {
    await harness.Bus.Publish<IOrderCreatedByUser>(
      new
      {
        OrderId = orderId,
        Products = new OrderProductDTO[]
        {
          new()
          {
            ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"),
            Quantity = 1,
            Price = 1000,
          },
          new()
          {
            ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"),
            Quantity = 2,
            Price = 100,
          },
        },
      }
    );
  }

  private static void AddNewOrderWithId(Guid orderId, OrderDbContext orders)
  {
    var order = new Order() { OrderId = orderId, CurrencyISO = "eur" };
    orders.Orders.Add(order);
    orders.SaveChanges();
  }

  private static ServiceProvider SetupServiceProvider()
  {
    return new ServiceCollection()
      .AddMassTransitTestHarness(o =>
      {
        _ = o.AddSagaStateMachine<NewOrderSaga, OrderState>().InMemoryRepository();
        _ = o.AddSagaStateMachine<CancelOrderSaga, CancelOrderState>().InMemoryRepository();
        o.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .AddDbContext<OrderDbContext>(builder =>
      {
        builder.UseInMemoryDatabase("cancelOrderSagaOrders");
        builder.EnableSensitiveDataLogging();
        builder.LogTo(Console.WriteLine, LogLevel.Warning);
      }
      // ServiceLifetime.Singleton
      )
      .AddLogging(l =>
      {
        l.AddConsole();
      })
      .BuildServiceProvider(true);
  }

  [Fact]
  public async Task CancelOrderSaga_SagaInitiationOnOrderCancellationRequestEvent_CancelSagaInstanceCreated()
  {
    var orderId = Guid.NewGuid();

    await _harness.Bus.Publish<IOrderCancellationRequest>(new { orderId });
    await Task.Delay(500); // makes sure events are fired and processed

    var doesInstanceExists = await _cancelSagasHarness.Sagas.Any(s => s.CorrelationId == orderId);
    Assert.True(doesInstanceExists);
    var sagaInstance = await _cancelSagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "Pending";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
  }

  [Fact]
  public async Task CancelOrderSaga_CancellableOrder_RemoveProductsReservationCommandPublished()
  {
    var orderId = Guid.NewGuid();
    var scope = _provider.CreateScope();
    using var orders = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    AddNewOrderWithId(orderId, orders);

    await _harness.Bus.Publish<IOrderCancellationRequest>(new { orderId });
    await Task.Delay(500); // makes sure events are fired and processed

    var doesInstanceExists = await _harness.Published.Any(s =>
      s.GetType() == typeof(OrderCancelledRemoveProductsReservationCommand)
    );
    var sagaInstance = await _cancelSagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "Pending";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
  }

  [Fact]
  public async Task CancelOrderSaga_CancellableOrder_OrderCancelled()
  {
    var orderId = Guid.NewGuid();
    var scope = _provider.CreateScope();
    var orders = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    AddNewOrderWithId(orderId, orders);
    await _harness.Bus.Publish<IOrderCancellationRequest>(new { orderId });

    await _harness.Bus.Publish<IOrderCancelledRemovedProductsReservation>(new { orderId });
    await Task.Delay(1500); // makes sure events are fired and processed

    var sagaInstance = await _cancelSagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "Confirmed";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
    var order = await orders.Orders.FindAsync(orderId);
    Assert.Equal(OrderStatus.Type.Cancelled, order?.Status);
  }

  [Fact]
  public async Task CancelOrderSaga_OrderCancelledCancellationUnavailable_OrderNotCancelled()
  {
    var orderId = Guid.NewGuid();
    var scope = _provider.CreateScope();
    using var orders = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    AddNewOrderWithId(orderId, orders);
    await _harness.Bus.Publish<IOrderCancellationRequest>(new { orderId });

    await _harness.Bus.Publish<IOrderCancelledCancellationUnavailable>(new { orderId });
    await Task.Delay(500); // makes sure events are fired and processed

    var sagaInstance = await _cancelSagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "Final";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
    var order = orders.Orders.Find(orderId);
    Assert.NotEqual(OrderStatus.Type.Cancelled, order?.Status);
  }
}
