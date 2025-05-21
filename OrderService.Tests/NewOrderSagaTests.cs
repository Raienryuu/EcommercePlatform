using Contracts;
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
  public async Task NewOrderSaga_SagaInitiationOnOrderCreatedByUserEvent_NewSagaInstanceCreated()
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

    var doesInstanceExists = await sagasHarness.Sagas.Any(s => s.CorrelationId == orderId);
    Assert.True(doesInstanceExists);
    var sagaInstance = await sagasHarness.Sagas.SelectAsync(s => s.CorrelationId == orderId).First();
    const string EXPECTED_STATE = "InCheckout";
    Assert.Equal(EXPECTED_STATE, sagaInstance.Saga.CurrentState);
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
      .AddSingleton<OrderDbContext, FakeOrderDbContext>()
      .AddSingleton<DbContextOptions>(static x =>
        new DbContextOptionsBuilder<FakeOrderDbContext>()
          .UseInMemoryDatabase($"orderSagas-{Guid.NewGuid()}")
          .Options
      )
      .BuildServiceProvider(true);
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    var orderId = Guid.NewGuid();
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
            Price = 850,
          },
          new()
          {
            ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"),
            Quantity = 2,
            Price = 50,
          },
        },
      }
    );
    await harness.Bus.Publish<IOrderPriceCalculated>(
      new
      {
        OrderId = orderId,
        CurrencyISO = "eur",
        TotalPriceInSmallestCurrencyUnit = 921312,
      }
    );

    await Task.Delay(500);
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
    _ = db.Orders.Add(
      new Order
      {
        OrderId = orderId,
        CurrencyISO = "eur",
        Delivery = new OrderDelivery()
        {
          CustomerInformation = new()
          {
            City = "Cityname",
            Address = "Adresso 5",
            Email = "thats@mail.com",
            Country = "Countryman",
            FullName = "Joe Doe",
            PhoneNumber = "+1324 231415",
            ZIPCode = "34512",
          },
          HandlerName = "dhl",
          Price = 0,
          PaymentType = PaymentType.Online,
          DeliveryType = DeliveryType.DirectCustomerAddress,
          Name = "Standard shipping",
        },
      }
    );
    _ = db.SaveChanges();
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
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
            Price = 1234,
          },
          new()
          {
            ProductId = Guid.Parse("86f94847-a8e4-4abc-8152-85d78d64bfd1"),
            Quantity = 2,
            Price = 4321,
          },
        },
      }
    );
    await harness.Bus.Publish<IOrderPriceCalculated>(
      new
      {
        OrderId = orderId,
        CurrencyISO = "eur",
        TotalPriceInSmallestCurrencyUnit = 921312,
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

    var freshOrder = await db.Orders.FindAsync(orderId);
    Assert.Equal(OrderStatus.Type.Cancelled, freshOrder!.Status);
    Assert.True(instanceConfirmed! == orderId);
  }

  [Fact]
  public async Task NewOrderSaga_SentCalculateTotalCostCommand_OrderTotalIsNotNull()
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
      .BuildServiceProvider(false);
    var order = new Order
    {
      OrderId = Guid.NewGuid(),
      CurrencyISO = "eur",
      Products =
      [
        new()
        {
          ProductId = Guid.NewGuid(),
          Price = 4441,
          Quantity = 2,
        },
      ],
      Delivery = new OrderDelivery()
      {
        CustomerInformation = new()
        {
          City = "Cityname",
          Address = "Adresso 5",
          Email = "thats@mail.com",
          Country = "Countryman",
          FullName = "Joe Doe",
          PhoneNumber = "+1324 231415",
          ZIPCode = "34512",
        },
        DeliveryId = Guid.NewGuid(),
        HandlerName = "dhl",
        Price = 0,
        PaymentType = PaymentType.Online,
        DeliveryType = DeliveryType.DirectCustomerAddress,
        Name = "Standard shipping",
      },
    };
    var db = provider.GetRequiredService<OrderDbContext>();
    _ = db.Orders.Add(order);
    _ = await db.SaveChangesAsync();
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    var orderProductsAsDto = order
      .Products.Select(p => new OrderProductDTO
      {
        ProductId = p.ProductId,
        Price = p.Price,
        Quantity = p.Quantity,
      })
      .ToArray();
    const int EXPECTED_TOTAL_PRICE = 4441 * 2;
    await harness.Bus.Publish<IOrderCreatedByUser>(new { order.OrderId, Products = orderProductsAsDto });

    await harness.Bus.Publish(
      new OrderCalculateTotalCostCommand
      {
        OrderId = order.OrderId,
        CurrencyISO = order.CurrencyISO,
        EurToCurrencyMultiplier = 1,
        Products = orderProductsAsDto,
        DeliveryId = order.Delivery.DeliveryId,
      }
    );
    await harness.Bus.Publish<IOrderPriceCalculated>(
      new
      {
        order.OrderId,
        order.CurrencyISO,
        TotalPriceInSmallestCurrencyUnit = EXPECTED_TOTAL_PRICE,
      }
    );

    var responseConsumed = await harness.Consumed.Any(m => m.MessageType == typeof(IOrderPriceCalculated));
    Assert.True(responseConsumed);
    Assert.Equal(EXPECTED_TOTAL_PRICE, order!.TotalPriceInSmallestCurrencyUnit);
  }
}
