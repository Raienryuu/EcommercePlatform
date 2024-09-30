using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Models.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityService.Controller
{
  [Route("api/v1/user")]
  [ApiController]
  public class UserController : ControllerBase
  {
	private readonly ApplicationDbContext _db;
	private readonly UserManager<IdentityUser<Guid>> _userManager;
	private readonly SignInManager<IdentityUser<Guid>> _signinManager;
	private readonly RoleManager<IdentityRole<Guid>> _roleManager;
	private readonly IConfiguration _configuration;

	public UserController(ApplicationDbContext db,
		UserManager<IdentityUser<Guid>> userManager,
		SignInManager<IdentityUser<Guid>> signInManager,
		RoleManager<IdentityRole<Guid>> roleManager,
		IConfiguration configuration)
	{
	  _db = db;
	  _userManager = userManager;
	  _signinManager = signInManager;
	  _roleManager = roleManager;
	  _configuration = configuration;
	}

	[HttpPost]
	[Route("register")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[AllowAnonymous]
	public async Task<ActionResult> RegisterNewUser([FromBody] NewUser registrationData)
	{
	  PasswordHasher<IdentityUser<Guid>> passwordHasher = new();
	  Debug.WriteLine(registrationData.Name);
	  bool isValid = new NewUserValidator().Validate(registrationData);

	  if (!isValid)
	  {
		return BadRequest("Invalid user data");
	  }

	  IdentityUser<Guid> newUser = new()
	  {
		UserName = registrationData.UserName,
		Email = registrationData.Email,
		PhoneNumber = registrationData.PhoneNumber,
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
	  var addedToRole = await _userManager.AddToRoleAsync(newUser, "User");

	  if (createdSuccessfully.Succeeded && addedToRole.Succeeded)
	  {
		UserAddress userAddress = UserAddress.CreateFrom(registrationData, newUser);
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
	public async Task<ActionResult> Login(
	  [FromBody] UserCredentials credentials)
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

	  var key = new SymmetricSecurityKey(
		Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
	  var tokenCredentials = new SigningCredentials(
		key, SecurityAlgorithms.HmacSha256);

	  var claims = new[]
	  {
		new Claim(ClaimTypes.Name, user.Id.ToString()),
		new Claim(ClaimTypes.Email, user.Email),
		new Claim(ClaimTypes.Role, string.Join(',',
		  await _userManager.GetRolesAsync(user))),
	  };

	  var tokenData = new JwtSecurityToken(_configuration["Jwt:Issuer"],
		_configuration["Jwt:Issuer"],
		claims,
		expires: DateTime.Now.AddDays(double.Parse(_configuration[key: "Jwt:ExpireTimeInDays"])),
		signingCredentials: tokenCredentials);

	  var jwToken = new JwtSecurityTokenHandler().WriteToken(tokenData);

	  jwToken = WebUtility.HtmlEncode("Bearer " + jwToken);

	  string response = $"{{ \"Authorization\" : \"{jwToken}\" }}";
	  return Ok(response);
	}

	[HttpGet]
	[Route("test")]
	[Authorize]
	public IActionResult AuthUser([FromHeader(Name = "UserId")] Guid userId)
	{

	  Console.WriteLine($"This method started: {userId}");


	  return Ok(userId);
	}
  }
}


