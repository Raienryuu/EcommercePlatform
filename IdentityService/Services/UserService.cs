using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Humanizer;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services;

public class UserService(
  ApplicationDbContext db,
  UserManager<IdentityUser<Guid>> userManager,
  IConfiguration configuration,
  ILogger<UserService> logger
) : IUserService
{
  public async Task<(bool isSucccess, IEnumerable<IdentityError> errorList)> RegisterNewUser(
    NewUser registrationData
  )
  {
    PasswordHasher<IdentityUser<Guid>> passwordHasher = new();

    var newUser = MapToNewUser(registrationData);

    newUser.PasswordHash = passwordHasher.HashPassword(newUser, registrationData.Password);
    var createdSuccessfully = await userManager.CreateAsync(newUser);
    if (createdSuccessfully.Succeeded == false)
    {
      return (false, createdSuccessfully.Errors);
    }

    var addedToRole = await userManager.AddToRoleAsync(newUser, "User");

    if (createdSuccessfully.Succeeded && addedToRole.Succeeded)
    {
      var userAddress = UserAddress.CreateFrom(registrationData, newUser);
      await db.Addresses.AddAsync(userAddress);
      await db.SaveChangesAsync();

      logger.LogInformation("Registerd new user");

      return (true, []);
    }

    return (false, []);
  }

  public async Task<(bool isSuccess, string message)> LogInUser(UserCredentials credentials)
  {
    var user = await userManager.FindByNameAsync(credentials.Login);

    if (user is null)
    {
      return (false, "{\"message\":\"Not able to get matching values from database.\"}");
    }

    var result = await userManager.CheckPasswordAsync(user, credentials.Password);
    if (!result)
    {
      logger.LogInformation("User {User} used invalid password", user.Id);
      return (false, "{\"message\":\"Not able to get matching values from database.\"}");
    }
    var jwtSection = configuration.GetRequiredSection("Jwt");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key")!));
    var tokenCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    /*var roles = await _userManager.GetRolesAsync(user);*/
    /**/
    /*if (roles.Count == 0)*/
    /*{*/
    /*  throw new InvalidOperationException("Trying to login user with no roles");*/
    /*}*/

    var claims = new[] { new Claim("UserId", user.Id.ToString()), new Claim("Role", "User") };

    var tokenData = new JwtSecurityToken(
      jwtSection.GetValue<string>("Issuer")!,
      jwtSection.GetValue<string>("Issuer")!,
      claims,
      expires: DateTime.Now.AddDays(jwtSection.GetValue<int>("ExpireTimeInDays")),
      signingCredentials: tokenCredentials
    );

    var jwToken = new JwtSecurityTokenHandler().WriteToken(tokenData);

    jwToken = WebUtility.HtmlEncode("Bearer " + jwToken);

    var response = $"{{ \"Authorization\" : \"{jwToken}\" }}";

    logger.LogInformation("User {User} logged in successfully", user.Id);

    return (true, response);
  }

  public async Task<(bool isSuccess, string username)> GetUsernameForLoggedUser(Guid userId)
  {
    var username = await db.Users.Where(x => x.Id == userId).Select(x => x.UserName).FirstAsync();
    if (username is null)
    {
      return (false, string.Empty);
    }
    username = username.ToLower().Pascalize();
    return (true, username);
  }

  private static IdentityUser<Guid> MapToNewUser(NewUser registrationData)
  {
    return new IdentityUser<Guid>()
    {
      Id = Guid.Empty,
      UserName = registrationData.UserName,
      Email = registrationData.Address.Email,
      PhoneNumber = registrationData.Address.PhoneNumber,
      EmailConfirmed = true,
      PhoneNumberConfirmed = true,
      TwoFactorEnabled = false,
      LockoutEnabled = true,
    };
  }
}
