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
using System.Collections.Immutable;

namespace IdentityService.Tests.Unit;

public class UserControllerTests
{
    private readonly UserManager<IdentityUser> _userManagerMock;
    private readonly UserController _cut;
    private readonly IConfiguration _configuration;

    public UserControllerTests()
    {
        var dbContext = new ApplicationDbContextFake();
        var store = Mock.Of<IUserStore<IdentityUser>>();
        var optionsAccessor = Mock.Of<IOptions<IdentityOptions>>();
        var passwordHasher = Mock.Of<IPasswordHasher<IdentityUser>>();
        var userValidators = Array.Empty<IUserValidator<IdentityUser>>();
        var passwordValidators = Array.Empty<IPasswordValidator<IdentityUser>>();
        var keyNormalizer = Mock.Of<ILookupNormalizer>();
        var errors = Mock.Of<IdentityErrorDescriber>();
        var services = Mock.Of<IServiceProvider>();
        var logger = NullLogger<UserManager<IdentityUser>>.Instance;

        _userManagerMock = new Mock<UserManager<IdentityUser>>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger).Object;

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Unit.Tests.json")
            .Build();
        _cut = new UserController(
          dbContext,
          _userManagerMock,
          null!,
          null!,
          _configuration);
    }

    [Fact]
    public async Task RegisterNewUser_ValidRegistrationUserData_UserRegisteredSuccessfully()
    {
        Mock.Get(_userManagerMock)
          .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>()))
          .Returns(Task.FromResult(IdentityResult.Success));

        Mock.Get(_userManagerMock)
          .Setup(x => x.AddToRoleAsync(
        It.IsAny<IdentityUser>(),
        It.IsAny<string>()))
          .Returns(Task.FromResult(IdentityResult.Success));
        NewUser user = SampleUserData.newUser;

        var result = await _cut.RegisterNewUser(user);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_BearerToken()
    {
        Mock.Get(_userManagerMock)
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(await Task.FromResult(SampleUserData.identityUser));
        Mock.Get(_userManagerMock)
            .Setup(x => x.CheckPasswordAsync(
        It.IsAny<IdentityUser>(),
     It.IsAny<string>()))
            .ReturnsAsync(await Task.FromResult(true));
        Mock.Get(_userManagerMock)
            .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(await Task.FromResult(new List<string>() { "User", "Admin" } as IList<string>));
        //Mock.Get(_configuration)
        //    .Setup(x => x.GetValue<string>(It.Is<string>(s => s == "Jwt:Key")))
        //    .Returns("1234567890123unittest4567890123456789012");
        //Mock.Get(_configuration)
        //    .Setup(x => x.GetValue<string>(It.Is<string>(s => s == "Jwt:Issuer")))
        //    .Returns("unittests.net");
        //Mock.Get(_configuration)
        //    .Setup(x => x.GetValue<string>(It.Is<string>(s => s == "Jwt:ExpireTimeInDays")))
        //    .Returns("30");
        UserCredentials userCredentials = SampleUserData.userCredentials;

        var result = await _cut.Login(userCredentials);

        string token = (string)((OkObjectResult)result).Value!;
        Assert.Contains("Bearer", token);
    }
}