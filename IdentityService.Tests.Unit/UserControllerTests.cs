using IdentityService.Controller;
using IdentityService.Tests.Unit.Fakes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityService.Tests.Unit;

public class UserControllerTests
{
  [Fact]
  public async Task RegisterNewUser_ValidRegistrationUserData_UserRegisteredSuccessfully()
  {
    var dbContext = new ApplicationDbContextFake();
    var userManagerMock = new UserManager<IdentityUser>(
      Mock.Of<IUserStore<IdentityUser>>(),
      Mock.Of<IOptions<IdentityOptions>>(),
      Mock.Of<IPasswordHasher<IdentityUser>>(),
      Mock.Of<IEnumerable<IUserValidator<IdentityUser>>>(),
      Mock.Of<IEnumerable<IPasswordValidator<IdentityUser>>>(),
      Mock.Of<ILookupNormalizer>(),
      Mock.Of<IdentityErrorDescriber>(),
      Mock.Of<IServiceProvider>(),
      Mock.Of<ILogger<UserManager<IdentityUser>>>()
      );
    var configurationMock = Mock.Of<IConfiguration>();
    var _cut = new UserController(
      dbContext,
      userManagerMock,
      null,
      null,
      configurationMock);


  }
}