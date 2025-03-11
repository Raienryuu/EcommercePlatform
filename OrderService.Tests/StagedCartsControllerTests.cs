using System.Net;
using OrderService.Models;

namespace OrderService.Tests;

public class StagedCartsTests(AppFactory app) : AppStartup(app)
{
  private const string API_URL = "http://localhost/api/";
  private const string USERID_HEADER_NAME = "userID";

  [Fact]
  public async Task CreateStagedCart_CartWithProducts_StagedCart()
  {
    var cart = new StageCartRequest()
    {
      Products = [new OrderProduct { ProductId = Guid.NewGuid(), Quantity = 4 }],
    };
    var httpRequest = GetValidPostRequest(cart);
    var res = await _client.SendAsync(httpRequest);
    var content = await res.Content.ReadFromJsonAsync<StagedCart>();

    Assert.True(res.IsSuccessStatusCode);
    Assert.NotNull(content);
    Assert.True(content.ValidUntil > DateTime.UtcNow);
  }

  private static HttpRequestMessage GetValidPostRequest(StageCartRequest cart)
  {
    var httpRequest = new HttpRequestMessage
    {
      Content = JsonContent.Create(cart),
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(API_URL + "v1/stagedcarts").Uri,
    };
    httpRequest.Headers.Add(USERID_HEADER_NAME, Guid.NewGuid().ToString());
    return httpRequest;
  }

  [Fact]
  public async Task GetStagedCart_ValidOwnerId_StagedCart()
  {
    var cart = new StageCartRequest()
    {
      Products = [new OrderProduct { ProductId = Guid.NewGuid(), Quantity = 8 }],
    };
    var postHttpRequest = GetValidPostRequest(cart);
    var res = await _client.SendAsync(postHttpRequest);
    var oldContent = await res.Content.ReadFromJsonAsync<StagedCart>();
    var getHttpRequest = GetValidGetRequest(oldContent!.OwnerId);

    res = await _client.SendAsync(getHttpRequest);
    var newContent = await res.Content.ReadFromJsonAsync<StagedCart>();

    Assert.True(res.IsSuccessStatusCode);
    Assert.NotNull(newContent);
    Assert.True(newContent.OwnerId == oldContent!.OwnerId);
    Assert.True(oldContent.Products.First().Quantity == newContent.Products.First().Quantity);
  }

  private static HttpRequestMessage GetValidGetRequest(Guid userId)
  {
    var getHttpRequest = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = new UriBuilder(API_URL + "v1/stagedcarts").Uri,
    };
    getHttpRequest.Headers.Add(USERID_HEADER_NAME, userId.ToString());
    return getHttpRequest;
  }

  [Fact]
  public async Task GetStagedCart_UserWithNoSavedCart_NotFoundResult()
  {
    var getHttpRequest = GetValidGetRequest(Guid.NewGuid());

    var res = await _client.SendAsync(getHttpRequest);

    Assert.False(res.IsSuccessStatusCode);
    Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
  }

  [Fact]
  public async Task GetStagedCart_NoUserIdHeader_ForbiddenError()
  {
    var res = await _client.GetAsync("api/v1/stagedCarts");

    Assert.False(res.IsSuccessStatusCode);
    Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
  }
}
