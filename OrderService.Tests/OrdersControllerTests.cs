using OrderService.Models;
using OrderService.Tests.Fakes;

namespace OrderService.Tests;

public class OrdersControllerTests
{
  [Fact]
  public async Task PlaceOrder_Order_201CreatedAt()
  {
	using var app = new App();
	using var client = app.CreateClient();
	var order = new Order()
	{
	  OrderId = Guid.NewGuid(),
	  UserId = 1,
	  Products = [
		new OrderProduct { ProductId = 5, Quantity = 1 }
		]
	};

	var res = await client.PostAsJsonAsync("api/v1/orders", order);

	Assert.True(res.StatusCode == System.Net.HttpStatusCode.Created);
  }

  [Fact]
  public async Task PostOrder_OrderWithEmptyProducts_ValidationError()
  {
	using var app = new App();
	using var client = app.CreateClient();
	var order = new Order()
	{
	  OrderId = Guid.NewGuid(),
	  UserId = 1,
	  Products = []
	};

	var res = await client.PostAsJsonAsync("api/v1/orders", order);
	var msg = await res.Content.ReadAsStringAsync();

	Assert.Contains("empty", msg);
  }

  [Fact]
  public async Task GetUserOrders_UserId_ListOfOrders()
  {
	using var app = new App();
	using var client = app.CreateClient();
	const int userId = 1;

	var res = await client.GetAsync($"api/v1/orders/{userId}");

	var list = await res.Content.ReadFromJsonAsync<List<Order>>();
	Assert.IsType<List<Order>>(list);
  }

  [Fact]
  public async Task GetOrder_ExistingOrderId_Order()
  {
	using var app = new App();
	var dbContext = app.Services.CreateScope().ServiceProvider.GetService<OrderDbContext>();
	FakeOrderDataInserter.FillData(dbContext!);
	using var client = app.CreateClient();

	Guid orderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");

	var res = await client.GetAsync($"api/v1/orders/{orderId}");

	var order = await res.Content.ReadFromJsonAsync<Order>();
	Assert.IsType<Order>(order);
  }
}