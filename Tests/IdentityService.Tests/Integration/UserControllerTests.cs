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
    var user = SampleUserData.NewUser;

    var result = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));

    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
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
}
