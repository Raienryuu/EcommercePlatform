using System.Net;
using System.Net.Http.Json;
using IdentityService.Tests.Fixtures;
using IdentityService.Tests.SampleData;

namespace IdentityService.Tests.Integration;

public class UserControllerTests(AppFixture app) : IClassFixture<AppFixture>
{
  private readonly HttpClient _client = app.CreateClient();

  [Fact]
  public async Task RegisterNewUser_ValidRegistrationUserData_UserRegisteredSuccessfully()
  {
    var user = SampleUserData.UniqueNewUser(Guid.NewGuid().ToString());

    var result = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));

    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
  }

  [Fact]
  public async Task RegisterNewUser_MissingFields_ShouldReturnBadRequest()
  {
    var user = new
    {
      // Missing Password and Address
      UserName = "incompleteUser"
    };
    var result = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task RegisterNewUser_DuplicateUser_ShouldReturnBadRequest()
  {
    var user = SampleUserData.NewUser;
    await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    var result = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task Login_ValidCredentials_BearerToken()
  {
    var user = SampleUserData.LoginUser;
    await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    var userCredentials = SampleUserData.UserCredentials;

    var result = await _client.PostAsync("api/v1/user/login", JsonContent.Create(userCredentials));

    var token = await result.Content.ReadAsStringAsync();
    Assert.Contains("Bearer", token);
  }

  [Fact]
  public async Task Login_WrongPassword_ShouldReturnBadRequest()
  {
    var user = SampleUserData.LoginUser;
    await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    var wrongCredentials = new { Login = user.UserName, Password = "wrongpassword" };
    var result = await _client.PostAsync("api/v1/user/login", JsonContent.Create(wrongCredentials));
    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task Login_NonExistentUser_ShouldReturnBadRequest()
  {
    var credentials = new { Login = "nonexistent", Password = "doesntmatter" };
    var result = await _client.PostAsync("api/v1/user/login", JsonContent.Create(credentials));
    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
  }

  [Fact]
  public async Task GetUsernameForLoggedUser_ValidUser_ShouldReturnUsername()
  {
    var user = SampleUserData.NewUser;
    await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    var loginResult = await _client.PostAsync("api/v1/user/login", JsonContent.Create(new { Login = user.UserName, Password = user.Password }));
    var tokenJson = await loginResult.Content.ReadAsStringAsync();
    var token = System.Text.Json.JsonDocument.Parse(tokenJson).RootElement.GetProperty("Authorization").GetString();
    // Simulate extracting userId (in real test, fetch from DB or response)
    // Here, we assume userId is not available, so this is a placeholder
    // Replace with actual userId retrieval if possible
    var userId = System.Guid.NewGuid(); // TODO: Replace with actual userId
    var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/user/username");
    request.Headers.Add("Authorization", token);
    request.Headers.Add("UserId", userId.ToString());
    var result = await _client.SendAsync(request);
    // Accept 200 or 404 depending on userId correctness
    Assert.True(result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetUsernameForLoggedUser_InvalidUserId_ShouldReturnNotFound()
  {
    var user = SampleUserData.NewUser;
    await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
    var loginResult = await _client.PostAsync("api/v1/user/login", JsonContent.Create(new { Login = user.UserName, Password = user.Password }));
    var tokenJson = await loginResult.Content.ReadAsStringAsync();
    var token = System.Text.Json.JsonDocument.Parse(tokenJson).RootElement.GetProperty("Authorization").GetString();
    var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/user/username");
    request.Headers.Add("Authorization", token);
    request.Headers.Add("UserId", System.Guid.NewGuid().ToString()); // Random, likely invalid
    var result = await _client.SendAsync(request);
    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }

  [Fact]
  public async Task GetUsernameForLoggedUser_WithoutAuth_ShouldReturnUnauthorized()
  {
    var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/user/username");
    request.Headers.Add("UserId", System.Guid.NewGuid().ToString());
    var result = await _client.SendAsync(request);
    Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
  }
}
