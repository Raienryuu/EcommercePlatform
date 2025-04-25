using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Humanizer;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Models.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityService.Controllers.V1;

[Route("api/v1/user")]
[ApiController]
public class UserController(
  ApplicationDbContext db,
  UserManager<IdentityUser<Guid>> userManager,
  RoleManager<IdentityRole<Guid>> roleManager,
  IConfiguration configuration
) : ControllerBase
{
  private readonly ApplicationDbContext _db = db;
  private readonly UserManager<IdentityUser<Guid>> _userManager = userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
  private readonly IConfiguration _configuration = configuration;

  [HttpPost]
  [Route("register")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [AllowAnonymous]
  public async Task<ActionResult> RegisterNewUser([FromBody] NewUser registrationData)
  {
    PasswordHasher<IdentityUser<Guid>> passwordHasher = new();
    var isValid = new NewUserValidator().Validate(registrationData);

    if (!isValid)
    {
      return BadRequest("Invalid user data");
    }

    IdentityUser<Guid> newUser = new()
    {
      UserName = registrationData.UserName,
      Email = registrationData.Address.Email,
      PhoneNumber = registrationData.Address.PhoneNumber,
      EmailConfirmed = true,
      PhoneNumberConfirmed = true,
      TwoFactorEnabled = false,
      LockoutEnabled = true,
    };

    newUser.PasswordHash = passwordHasher.HashPassword(newUser, registrationData.Password);
    var createdSuccessfully = await _userManager.CreateAsync(newUser);
    if (createdSuccessfully.Succeeded == false)
    {
      return BadRequest(createdSuccessfully.Errors);
    }
    var userRole = await _roleManager.RoleExistsAsync("User");
    if (userRole == false)
    {
      throw new InvalidOperationException("Trying to register user without having roles in database.");
    }
    var addedToRole = await _userManager.AddToRoleAsync(newUser, "User");

    if (createdSuccessfully.Succeeded && addedToRole.Succeeded)
    {
      var userAddress = UserAddress.CreateFrom(registrationData, newUser);
      await _db.Addresses.AddAsync(userAddress);
      await _db.SaveChangesAsync();
      return Ok();
    }
    else
    {
      return BadRequest("Unable to register a new user.");
    }
  }

  [HttpPost]
  [Route("login")]
  [ProducesResponseType(StatusCodes.Status202Accepted)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [AllowAnonymous]
  public async Task<ActionResult> Login([FromBody] UserCredentials credentials)
  {
    if (credentials is null)
    {
      return BadRequest("No user credentials have been supplied.");
    }

    var user = await _userManager.FindByNameAsync(credentials.Login);

    if (user is null)
    {
      return BadRequest("{\"message\":\"Not able to get matching values from database.\"}");
    }

    var result = await _userManager.CheckPasswordAsync(user, credentials.Password);
    if (!result)
    {
      return BadRequest("{\"message\":\"Not able to get matching values from database.\"}");
    }
    var jwtSection = _configuration.GetRequiredSection("Jwt");

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
    return Ok(response);
  }

  [HttpGet]
  [Route("username")]
  [Authorize]
  public async Task<IActionResult> AuthUser([FromHeader(Name = "UserId")] Guid userId)
  {
    var username = await db.Users.Where(x => x.Id == userId).Select(x => x.UserName).FirstAsync();
    if (username is null)
    {
      return NotFound();
    }
    username = username.ToLower().Pascalize();
    return Ok(JsonSerializer.Serialize(username));
  }
}
