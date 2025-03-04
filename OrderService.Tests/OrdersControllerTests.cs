using OrderService.Models;
using OrderService.Tests.Fakes;

namespace OrderService.Tests;

public class OrdersControllerTests(AppFactory app) : AppStartup(app)
{
  [Fact]
  public async Task GetUserOrders_UserId_ListOfOrders()
  {
    const int userId = 1;
    var res = await _client.GetAsync($"api/v1/orders/{userId}");

    var list = await res.Content.ReadFromJsonAsync<List<Order>>();
    Assert.IsType<List<Order>>(list);
  }

  [Fact]
  public async Task GetOrder_ExistingOrderId_Order()
  {
    var dbContext = _app.Services.CreateScope().ServiceProvider.GetService<OrderDbContext>();
    FakeOrderDataInserter.FillData(dbContext!);
    var orderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");

    var res = await _client.GetAsync($"api/v1/orders/{orderId}");

    var order = await res.Content.ReadFromJsonAsync<Order>();
    _ = Assert.IsType<Order>(order);
  }

  [Fact]
  public async Task PostOrder_Order_201CreatedAt()
  {
    var order = new Order()
    {
      OrderId = Guid.NewGuid(),
      UserId = 1,
      Products =
      [
        new OrderProduct { ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"), Quantity = 1 },
      ],
    };

    var res = await _client.PostAsJsonAsync("api/v1/orders", order);

    Assert.True(res.StatusCode == System.Net.HttpStatusCode.Created);
  }

  [Fact]
  public async Task PostOrder_OrderWithEmptyProducts_ValidationError()
  {
    var order = new Order()
    {
      OrderId = Guid.NewGuid(),
      UserId = 1,
      Products = [],
    };

    var res = await _client.PostAsJsonAsync("api/v1/orders", order);
    var msg = await res.Content.ReadAsStringAsync();

    Assert.Contains("empty", msg);
  }
}
