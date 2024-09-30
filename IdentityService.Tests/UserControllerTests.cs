using System.Net;
using System.Net.Http.Json;
using IdentityService.Tests.SampleData;

namespace IdentityService.Tests;

public class UserControllerTests : IClassFixture<AppFixture>
{
  private readonly HttpClient _client;

  public UserControllerTests(AppFixture app)
  {
	_client = app.CreateClient();
  }


  [Fact]
  public async Task RegisterNewUser_ValidRegistrationUserData_UserRegisteredSuccessfully()
  {
	var user = SampleUserData.newUser;

	var result = await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));

	Assert.Equal(HttpStatusCode.OK, result.StatusCode);
  }

  [Fact]
  public async Task Login_ValidCredentials_BearerToken()
  {
	var user = SampleUserData.loginUser;
	await _client.PostAsync("api/v1/user/register", JsonContent.Create(user));
	var userCredentials = SampleUserData.userCredentials;

	var result = await _client.PostAsync("api/v1/user/login", JsonContent.Create(userCredentials));

	string token = await result.Content.ReadAsStringAsync();
	Assert.Contains("Bearer", token);
  }
}