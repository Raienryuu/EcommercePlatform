using System.Net;
using System.Net.Http.Json;
using IdentityService.Models;
using IdentityService.Tests.Fixtures;
using IdentityService.Tests.SampleData;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.Tests.Integration;

public class AddressControllerTests(AppFixture app) : IClassFixture<AppFixture>
{
  private readonly HttpClient _client = app.CreateClient();

  private async Task<Guid> RegisterAndGetUserIdAsync()
  {
    var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
    var user = new NewUser
    {
      UserName = $"user_{unique}",
      Password = "passwd",
      Address = new UserAddress
      {
        FullName = "Tom Dodo",
        Email = $"dodoto_{unique}@mailtown.com",
        PhoneNumber = "+48823928132",
        Address = "2231 Oliver Street",
        City = "Fort Worth",
        ZIPCode = "76147",
        Country = "United States",
      },
    };
    var credentials = new UserCredentials { Login = user.UserName, Password = user.Password };
    var regResponse = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    regResponse.EnsureSuccessStatusCode();
    var loginResponse = await _client.PostAsync("api/v1/user/login", JsonContent.Create(credentials));
    loginResponse.EnsureSuccessStatusCode();
    var tokenJson = await loginResponse.Content.ReadAsStringAsync();
    var tokenStr = tokenJson.Split('"')[3].Replace("Bearer ", "");
    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(tokenStr);
    var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "UserId");
    return Guid.Parse(userIdClaim!.Value);
  }

  [Fact]
  public async Task GetAddresses_ReturnsAddressesForUser()
  {
    var userId = await RegisterAndGetUserIdAsync();
    var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/addresses");
    request.Headers.Add("UserId", userId.ToString());
    var response = await _client.SendAsync(request);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task PostAddress_ValidAddress_CreatesAddress()
  {
    var userId = await RegisterAndGetUserIdAsync();
    var address = SampleUserData.SampleAddress;
    address.UserId = userId;
    var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/addresses");
    request.Headers.Add("UserId", userId.ToString());
    request.Content = JsonContent.Create(address);
    var response = await _client.SendAsync(request);
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    if (response.IsSuccessStatusCode)
    {
      var created = await response.Content.ReadFromJsonAsync<UserAddress>();
      Assert.NotNull(created);
    }
  }

  [Fact]
  public async Task GetAddressById_ValidId_ReturnsAddress()
  {
    var userId = await RegisterAndGetUserIdAsync();
    var address = SampleUserData.SampleAddress;
    address.UserId = userId;
    var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/addresses");
    postRequest.Headers.Add("UserId", userId.ToString());
    postRequest.Content = JsonContent.Create(address);
    var postResponse = await _client.SendAsync(postRequest);
    Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
    if (!postResponse.IsSuccessStatusCode) return;
    var created = await postResponse.Content.ReadFromJsonAsync<UserAddress>();
    var getRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/addresses/{created!.Id}");
    getRequest.Headers.Add("UserId", userId.ToString());
    var getResponse = await _client.SendAsync(getRequest);
    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    if (getResponse.IsSuccessStatusCode)
    {
      var fetched = await getResponse.Content.ReadFromJsonAsync<UserAddress>();
      Assert.NotNull(fetched);
    }
  }

  [Fact]
  public async Task PutAddress_ValidUpdate_UpdatesAddress()
  {
    var userId = await RegisterAndGetUserIdAsync();
    var address = SampleUserData.SampleAddress;
    address.UserId = userId;
    var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/addresses");
    postRequest.Headers.Add("UserId", userId.ToString());
    postRequest.Content = JsonContent.Create(address);
    var postResponse = await _client.SendAsync(postRequest);
    Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
    if (!postResponse.IsSuccessStatusCode) return;
    var created = await postResponse.Content.ReadFromJsonAsync<UserAddress>();
    created!.City = "UpdatedCity";
    var putRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/addresses");
    putRequest.Headers.Add("UserId", userId.ToString());
    putRequest.Content = JsonContent.Create(created);
    var putResponse = await _client.SendAsync(putRequest);
    Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
    if (putResponse.IsSuccessStatusCode)
    {
      var updated = await putResponse.Content.ReadFromJsonAsync<UserAddress>();
      Assert.NotNull(updated);
      Assert.Equal("UpdatedCity", updated.City);
    }
  }

  [Fact]
  public async Task DeleteAddress_ValidId_DeletesAddress()
  {
    var userId = await RegisterAndGetUserIdAsync();
    var address = SampleUserData.SampleAddress;
    address.UserId = userId;
    var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/addresses");
    postRequest.Headers.Add("UserId", userId.ToString());
    postRequest.Content = JsonContent.Create(address);
    var postResponse = await _client.SendAsync(postRequest);
    Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
    if (!postResponse.IsSuccessStatusCode) return;
    var created = await postResponse.Content.ReadFromJsonAsync<UserAddress>();
    var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/addresses/{created!.Id}");
    deleteRequest.Headers.Add("UserId", userId.ToString());
    var deleteResponse = await _client.SendAsync(deleteRequest);
    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }
}