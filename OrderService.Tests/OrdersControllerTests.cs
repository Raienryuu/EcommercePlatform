using MassTransit;
using MassTransit.Testing;
using OrderService.MessageQueue.Sagas;
using OrderService.MessageQueue.Sagas.SagaStates;
using OrderService.Models;
namespace OrderService.Tests;

public class OrdersControllerTests
{
  [Fact]
  public async Task PlaceOrder_Order_201CreatedAt()
  {
	var order = new Order()
	{
	  OrderId = Guid.NewGuid(),
	  UserId = 1,
	  Products = [
		new OrderProduct { ProductId = 5, Quantity = 1 }
		]
	};
	var app = new App();
	using var client = app.CreateClient();

	var res = await client.PostAsJsonAsync("api/v1/orders", order);

	Assert.True(res.StatusCode == System.Net.HttpStatusCode.Created);
  }

  [Fact]
  public async Task PlaceOrder_OrderWithEmptyProducts_ValidationError()
  {
	var order = new Order()
	{
	  OrderId = Guid.NewGuid(),
	  UserId = 1,
	  Products = []
	};
	var app = new App();
	using var client = app.CreateClient();

	var res = await client.PostAsJsonAsync("api/v1/orders", order);
	var msg = await res.Content.ReadAsStringAsync();

	Assert.Contains("empty", msg);
  }

  
}