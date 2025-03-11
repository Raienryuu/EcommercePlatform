using System.Net;
using OrderService.Models;
using OrderService.Tests.Fakes;

namespace OrderService.Tests;

public class OrdersControllerTests(AppFactory app) : AppStartup(app)
{
  private readonly Uri _apiUrl = new UriBuilder("http://localhost/api/v1/orders").Uri;

  [Fact]
  public async Task GetUserOrders_UserId_ListOfOrders()
  {
    var userId = Guid.NewGuid();
    var httpRequestMessage = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = _apiUrl };
    httpRequestMessage.Headers.Add("userId", userId.ToString());

    var res = await _client.SendAsync(httpRequestMessage);
    var list = await res.Content.ReadFromJsonAsync<List<Order>>();

    _ = Assert.IsType<List<Order>>(list);
    Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    Assert.NotNull(list);
  }

  [Fact]
  public async Task GetOrder_ExistingOrderId_Order()
  {
    var dbContext = _app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderDbContext>();
    FakeOrderDataInserter.FillData(dbContext);
    var orderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(_apiUrl + $"/{orderId}").Uri,
    };
    request.Headers.Add("userId", "75699034-2ed6-4f39-a984-89bab648294c");

    var res = await _client.SendAsync(request);

    var order = await res.Content.ReadFromJsonAsync<Order>();
    _ = Assert.IsType<Order>(order);
  }

  [Fact]
  public async Task PostOrder_Order_201CreatedAt()
  {
    var order = new Order()
    {
      Products =
      [
        new OrderProduct { ProductId = Guid.Parse("7fd990db-c5e4-42f5-ab11-1d39ada007ab"), Quantity = 1 },
      ],
    };
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = _apiUrl,
      Content = JsonContent.Create(order),
    };
    request.Headers.Add("userId", Guid.NewGuid().ToString());

    var res = await _client.SendAsync(request);

    Assert.Equal(HttpStatusCode.Created, res.StatusCode);
  }

  [Fact]
  public async Task PostOrder_OrderWithEmptyProducts_ValidationError()
  {
    var order = new Order()
    {
      OrderId = Guid.NewGuid(),
      UserId = Guid.NewGuid(),
      Products = [],
    };
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = _apiUrl,
      Content = JsonContent.Create(order),
    };
    request.Headers.Add("userId", Guid.NewGuid().ToString());

    var res = await _client.SendAsync(request);
    var msg = await res.Content.ReadAsStringAsync();

    Assert.Contains("empty", msg);
  }
}
