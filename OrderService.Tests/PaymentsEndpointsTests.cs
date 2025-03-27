using System.Net;

namespace OrderService.Tests;

public class PaymentsEndpointsTests(AppFactory app) : IClassFixture<AppFactory>
{
  private readonly HttpClient _client = app.CreateClient();
  private readonly Uri _apiUrl = new UriBuilder("http://localhost/api/v1/payments").Uri;

  [Fact]
  public async Task CreateNewPaymentSession_OrderWithoutTotalPriceId_Accepted()
  {
    var orderId = Guid.Parse("3066ca92-207a-4333-909a-b257123791f7");
    var httpRequestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(_apiUrl + "/" + orderId).Uri,
    };
    httpRequestMessage.Headers.Add("UserId", "75699034-2ed6-4f39-a984-89bab648294c");

    var res = await _client.SendAsync(httpRequestMessage);

    Assert.Equal(HttpStatusCode.Accepted, res.StatusCode);
  }

  [Fact]
  public async Task CreateNewPaymentSession_OrderWithPriceId_PaymentIntent()
  {
    var orderId = Guid.Parse("f1d0373d-6b65-419d-9308-55eabe156d1a");
    var httpRequestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(_apiUrl + "/" + orderId).Uri,
    };
    httpRequestMessage.Headers.Add("UserId", "75699034-2ed6-4f39-a984-89bab648294c");

    var res = await _client.SendAsync(httpRequestMessage);
    var response = await res.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    Assert.Contains("secret", response);
  }

  [Fact]
  public async Task CreateNewPaymentSession_OrderWithStripeIdAssigned_OldSecret()
  {
    var orderId = Guid.Parse("091ae438-aa1d-42cf-8058-2f89fcf313e2");
    var httpRequestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Post,
      RequestUri = new UriBuilder(_apiUrl + "/" + orderId).Uri,
    };
    httpRequestMessage.Headers.Add("UserId", "75699034-2ed6-4f39-a984-89bab648294c");

    var res = await _client.SendAsync(httpRequestMessage);
    var response = await res.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    Assert.Contains("old", response);
  }
}
