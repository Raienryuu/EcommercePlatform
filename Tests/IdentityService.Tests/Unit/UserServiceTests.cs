using System.Threading;
using System.Threading.Tasks;
using Common;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services;
using IdentityService.Tests.Fakes;
using IdentityService.Tests.SampleData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityService.Tests.Unit;

public class UserServiceTests
{
  private readonly UserService _userService;
  private readonly ApplicationDbContextFake _dbContext;
  private readonly Mock<UserManager<IdentityUser<Guid>>> _userManagerMock;
  private readonly IConfiguration _configuration;
  private readonly Mock<ILogger<UserService>> _loggerMock;

  public UserServiceTests()
  {
    var connection = new SqliteConnection("datasource=:memory:");
    connection.Open();
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlite(connection)
        .Options;
    _dbContext = new ApplicationDbContextFake(options);
    _dbContext.Database.EnsureCreated();
    _userManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
        Mock.Of<IUserStore<IdentityUser<Guid>>>(), null!, null!, null!, null!, null!, null!, null!, null!);
    _configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
        {
          {"Jwt:Key", "testkeytestkeytestkeytestkeytestkey12"},
          {"Jwt:Issuer", "testissuer"},
          {"Jwt:ExpireTimeInDays", "1"}
        }
      )
      .Build();
    _loggerMock = new Mock<ILogger<UserService>>();
    _userService = new UserService(_dbContext, _userManagerMock.Object, _configuration, _loggerMock.Object);
    _dbContext.FillData();
  }

  [Fact]
  public async Task RegisterNewUser_ValidData_ReturnsSuccess()
  {
    var newUser = SampleUserData.NewUser;
    _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser<Guid>>()))
        .ReturnsAsync(IdentityResult.Success)
        .Callback<IdentityUser<Guid>>((user) =>
        {
          _dbContext.Users.Add(user);
          _dbContext.SaveChanges();
        });
    _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
        .ReturnsAsync(IdentityResult.Success);

    var result = await _userService.RegisterNewUser(newUser, CancellationToken.None);

    Assert.True(result.IsSuccess);
  }

  [Fact]
  public async Task LogInUser_ValidCredentials_ReturnsToken()
  {
    var credentials = SampleUserData.UserCredentials;
    var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), UserName = credentials.Login };
    _userManagerMock.Setup(m => m.FindByNameAsync(credentials.Login)).ReturnsAsync(user);
    _userManagerMock.Setup(m => m.CheckPasswordAsync(user, credentials.Password)).ReturnsAsync(true);

    var result = await _userService.LogInUser(credentials, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Contains("Bearer", result.Value);
  }

  [Fact]
  public async Task GetUsernameForLoggedUser_ValidUserId_ReturnsUsername()
  {
    var user = _dbContext.Users.First();

    var result = await _userService.GetUsernameForLoggedUser(user.Id, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(user.UserName, result.Value);
  }

  [Fact]
  public async Task RegisterNewUser_CreateAsyncFails_ReturnsError()
  {
      var newUser = SampleUserData.NewUser;
      var identityError = new IdentityError { Description = "Create failed" };
      var failedResult = IdentityResult.Failed(identityError);
      _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser<Guid>>()))
          .ReturnsAsync(failedResult);
      var result = await _userService.RegisterNewUser(newUser, CancellationToken.None);
      Assert.False(result.IsSuccess);
      Assert.Contains("Create failed", result.ErrorMessage);
  }

  [Fact]
  public async Task RegisterNewUser_AddToRoleFails_ReturnsError()
  {
      var newUser = SampleUserData.NewUser;
      _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser<Guid>>()))
          .ReturnsAsync(IdentityResult.Success)
          .Callback<IdentityUser<Guid>>((user) => {
              _dbContext.Users.Add(user);
              _dbContext.SaveChanges();
          });
      var identityError = new IdentityError { Description = "Role failed" };
      var failedResult = IdentityResult.Failed(identityError);
      _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
          .ReturnsAsync(failedResult);
      var result = await _userService.RegisterNewUser(newUser, CancellationToken.None);
      Assert.False(result.IsSuccess);
      Assert.Contains("Role failed", result.ErrorMessage);
  }

  [Fact]
  public async Task LogInUser_UserNotFound_ReturnsError()
  {
      var credentials = SampleUserData.UserCredentials;
      _userManagerMock.Setup(m => m.FindByNameAsync(credentials.Login)).ReturnsAsync((IdentityUser<Guid>?)null);
      var result = await _userService.LogInUser(credentials, CancellationToken.None);
      Assert.False(result.IsSuccess);
      Assert.Contains("Not able to get matching values", result.ErrorMessage);
  }

  [Fact]
  public async Task LogInUser_InvalidPassword_ReturnsError()
  {
      var credentials = SampleUserData.UserCredentials;
      var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), UserName = credentials.Login };
      _userManagerMock.Setup(m => m.FindByNameAsync(credentials.Login)).ReturnsAsync(user);
      _userManagerMock.Setup(m => m.CheckPasswordAsync(user, credentials.Password)).ReturnsAsync(false);
      var result = await _userService.LogInUser(credentials, CancellationToken.None);
      Assert.False(result.IsSuccess);
      Assert.Contains("Not able to get matching values", result.ErrorMessage);
  }

  [Fact]
  public async Task GetUsernameForLoggedUser_UserNotFound_ReturnsError()
  {
      var nonExistentUserId = Guid.NewGuid();
      var result = await _userService.GetUsernameForLoggedUser(nonExistentUserId, CancellationToken.None);
      Assert.False(result.IsSuccess);
      Assert.Contains("Couldn't find user with given Id", result.ErrorMessage);
  }
}