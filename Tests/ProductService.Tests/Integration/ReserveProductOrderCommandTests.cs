using MassTransit;
using MassTransit.Testing;
using MessageQueue.Contracts;
using MessageQueue.DTOs;
using Microsoft.EntityFrameworkCore;
using ProductService.MessageQueue.Consumers;
using ProductService.Tests.Fakes;

namespace ProductService.Tests.Integration;

public class ReserveProductOrderTests(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
  private static bool s_isStarted;
  private readonly object _lock = new();

  private void FillDatabase(ServiceProvider provider)
  {
    lock (_lock)
    {
      if (s_isStarted)
      {
        return;
      }
      using var scope = provider.CreateAsyncScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
      _ = dbContext.Database.EnsureCreated();
      new ProductDbContextFakeBuilder().FillWithTestData(dbContext);
      s_isStarted = true;
    }
  }

  [Fact]
  public async Task ReserveOrderProducts_ValidOrder_ProductsNotAvailable()
  {
    await using var provider = new ServiceCollection()
      .AddDbContext<ProductDbContext, ProductDbContextFake>(o => o.UseSqlServer(db.GetConnectionString()))
      .AddMassTransitTestHarness(o =>
      {
        _ = o.AddConsumer<ReserveOrderProductsConsumer>();
        o.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .BuildServiceProvider(true);
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();

    FillDatabase(provider);

    var orderId = Guid.NewGuid();
    await harness.Bus.Publish<ReserveOrderProductsCommand>(
      new()
      {
        OrderId = orderId,
        Products =
        [
          new OrderProductDTO
          {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            Price = 2524,
          },
        ],
      },
      context => context.CorrelationId = orderId
    );

    var orderCommandConsumed = await harness.Consumed.Any(x => x.Context.CorrelationId == orderId);
    var publishedResponse = harness.Published.Select(x => x.Context.CorrelationId == orderId).ToList().Last();
    Assert.True(orderCommandConsumed);
    _ = Assert.IsAssignableFrom<IOrderProductsNotAvailable>(publishedResponse.MessageObject);
  }

  [Fact]
  public async Task ReserveOrderProducts_ValidOrder_ProductsReserved()
  {
    await using var provider = new ServiceCollection()
      .AddDbContext<ProductDbContext, ProductDbContextFake>(o => o.UseSqlServer(db.GetConnectionString()))
      .AddMassTransitTestHarness(o =>
      {
        _ = o.AddConsumer<ReserveOrderProductsConsumer>();
        o.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
      })
      .BuildServiceProvider(true);
    var harness = provider.GetRequiredService<ITestHarness>();
    await harness.Start();
    FillDatabase(provider);

    var orderId = Guid.NewGuid();
    await harness.Bus.Publish<ReserveOrderProductsCommand>(
      new()
      {
        OrderId = orderId,
        Products =
        [
          new OrderProductDTO
          {
            ProductId = Guid.Parse("f12e47a0-82f9-4231-abce-63280e7d3d99"),
            Quantity = 2,
            Price = 8000,
          },
        ],
      },
      context => context.CorrelationId = orderId
    );

    await Task.Delay(1000); // required to mitigate flaky test results

    var orderCommandConsumed = await harness.Consumed.Any(x => x.Context.CorrelationId == orderId);
    var publishedResponse = harness.Published.Select(x => x.Context.CorrelationId == orderId).ToList().Last();
    Assert.True(orderCommandConsumed);
    _ = Assert.IsAssignableFrom<IOrderReserved>(publishedResponse.MessageObject);

    await using var scope = provider.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    var reservationRecord = await dbContext.OrdersReserved.FindAsync(orderId);
    Assert.NotNull(reservationRecord);
  }
}
