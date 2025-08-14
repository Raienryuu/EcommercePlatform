using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Common;
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
  public async Task<ServiceResult> RegisterNewUser(
    NewUser registrationData,
    CancellationToken cancellationToken = default
  )
  {
    PasswordHasher<IdentityUser<Guid>> passwordHasher = new();

    var newUser = MapToNewUser(registrationData);

    newUser.PasswordHash = passwordHasher.HashPassword(newUser, registrationData.Password);
    var createdSuccessfully = await userManager.CreateAsync(newUser);
    if (createdSuccessfully.Succeeded == false)
    {
      var errorMsg = string.Join(", ", createdSuccessfully.Errors.Select(e => e.Description));
      return ServiceResults.Error(errorMsg, HttpStatusCode.BadRequest);
    }

    var addedToRole = await userManager.AddToRoleAsync(newUser, "User");
    if (addedToRole.Succeeded == false)
    {
      var errorMsg = string.Join(", ", addedToRole.Errors.Select(e => e.Description));
      return ServiceResults.Error(errorMsg, HttpStatusCode.BadRequest);
    }

    var userAddress = UserAddress.CreateFrom(registrationData, newUser);
    await db.Addresses.AddAsync(userAddress, cancellationToken);
    await db.SaveChangesAsync(cancellationToken);

    logger.RegisteredNewUser(newUser.Id);

    return ServiceResults.Success(HttpStatusCode.OK);
  }

  public async Task<ServiceResult<string>> LogInUser(
    UserCredentials credentials,
    CancellationToken cancellationToken = default
  )
  {
    var user = await userManager.FindByNameAsync(credentials.Login);

    if (user is null)
    {
      return ServiceResults.Error("Not able to get matching values from database.", HttpStatusCode.BadRequest);
    }

    var isPasswordValid = await userManager.CheckPasswordAsync(user, credentials.Password);
    if (!isPasswordValid)
    {
      logger.InvalidLogin(user.Id);
      return ServiceResults.Error("Not able to get matching values from database.", HttpStatusCode.BadRequest);
    }

    var jwtSection = configuration.GetSection("Jwt");
    if (jwtSection == null)
    {
      throw new InvalidOperationException("Section 'Jwt' not found in configuration.");
    }

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

    logger.SuccessfullLogin(user.Id);

    return ServiceResults.Success(response, HttpStatusCode.OK);
  }

  public async Task<ServiceResult<string>> GetUsernameForLoggedUser(
    Guid userId,
    CancellationToken cancellationToken = default
  )
  {
    var username = await db
      .Users.Where(x => x.Id == userId)
      .Select(x => x.UserName)
      .FirstOrDefaultAsync(cancellationToken);
    if (username is null)
    {
      return ServiceResults.Error("Couldn't find user with given Id", HttpStatusCode.NotFound);
    }

    return ServiceResults.Success(username.ToLower().Pascalize(), HttpStatusCode.OK);
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
