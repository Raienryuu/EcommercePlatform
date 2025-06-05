using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using Microsoft.EntityFrameworkCore;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;
using OrderService.Services;
using OrderService.Tests.Fakes;

namespace OrderService.Tests;

public class CancelOrderSagaTests
{
  private readonly ServiceProvider _provider;
  private readonly ITestHarness _harness;

  private readonly ISagaStateMachineTestHarness<CancelOrderSaga, CancelOrderState> _cancelSagasHarness;

  public CancelOrderSagaTests()
  {
    _provider = SetupServiceProvider();
    _harness = _provider.GetRequiredService<ITestHarness>();
    _harness.Start().Wait();

    _cancelSagasHarness = _harness.GetSagaStateMachineHarness<CancelOrderSaga, CancelOrderState>();
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
      .AddDbContext<OrderDbContext>(
        builder =>
        {
          builder.UseInMemoryDatabase("cancel-order-saga-tests");
          builder.EnableSensitiveDataLogging();
          builder.LogTo(Console.WriteLine, LogLevel.Information);
        },
        ServiceLifetime.Singleton,
        ServiceLifetime.Singleton
      )
      .AddLogging(l => { })
      .AddSingleton<IStripePaymentService, FakeStripePaymentService>()
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
    orders.Database.EnsureCreated();
    AddNewOrderWithId(orderId, orders);
    await _harness.Bus.Publish<IOrderCancellationRequest>(new { orderId });

    await _harness.Bus.Publish<IOrderCancelledRemovedProductsReservation>(new { orderId });
    await Task.Delay(500); // makes sure events are fired and processed

    var sagaInstance = await _cancelSagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "Confirmed";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
    var order = await orders.Orders.FindAsync(orderId);
    Assert.NotNull(order);
    Assert.Equal(true, order?.IsCancelled);
  }
}
