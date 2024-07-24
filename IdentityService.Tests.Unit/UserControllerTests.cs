using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using IdentityService.Controller;
using IdentityService.Models;
using IdentityService.Tests.Unit.Fakes;
using IdentityService.Tests.Unit.SampleData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityService.Tests.Unit;

public class UserControllerTests : IClassFixture<AppFixture>
{
    private readonly UserManager<IdentityUser> _userManagerMock;
    private readonly UserController _cut;
    private readonly IConfiguration _configuration;

    private readonly HttpClient _client;

    public UserControllerTests(AppFixture app)
    {
        _client = app.CreateClient();
        // var dbContext = new ApplicationDbContextFake();
        // var store = Mock.Of<IUserStore<IdentityUser>>();
        // var optionsAccessor = Mock.Of<IOptions<IdentityOptions>>();
        // var passwordHasher = Mock.Of<IPasswordHasher<IdentityUser>>();
        // var userValidators = Array.Empty<IUserValidator<IdentityUser>>();
        // var passwordValidators = Array.Empty<IPasswordValidator<IdentityUser>>();
        // var keyNormalizer = Mock.Of<ILookupNormalizer>();
        // var errors = Mock.Of<IdentityErrorDescriber>();
        // var services = Mock.Of<IServiceProvider>();
        // var logger = NullLogger<UserManager<IdentityUser>>.Instance;

        // _userManagerMock = new Mock<UserManager<IdentityUser>>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger).Object;

        // _cut = new UserController(
        //   dbContext,
        //   _userManagerMock,
        //   null!,
        //   null!,
        //   _configuration);
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Unit.Tests.json")
            .Build();
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