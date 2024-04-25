using MassTransit;
using MassTransit.Testing;
using OrderService.Models;
namespace OrderService.Tests;

public class OrderServiceControllerUnitTests
{
  [Fact]
  public async Task PlaceOrder_Order_Ok200()
  {
    var order = new Order()
    {
      Id = 1,
      UserId = 1,
      Products = [new OrderProduct { Id = 5, Quantity = 1 }]
    };
    await using var provider = new ServiceCollection()
      .AddMassTransitTestHarness(o =>
      {

      })
      .BuildServiceProvider(true);


    var harness = provider.GetRequiredService<ITestHarness>();

    await harness.Start();
    Assert.True(1 == 0);
  }
}