using IdentityService.Models;
using IdentityService.Models.Validators;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers.V1;

[Route("api/v1/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
  [HttpPost]
  [Route("register")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [AllowAnonymous]
  public async Task<ActionResult> RegisterNewUser([FromBody] NewUser registrationData)
  {
    var isValid = new NewUserValidator().Validate(registrationData);

    if (!isValid)
    {
      return BadRequest("Invalid user data");
    }

    var (isSucccess, errorList) = await userService.RegisterNewUser(registrationData);

    if (isSucccess)
    {
      return Ok();
    }
    return BadRequest(errorList);
  }

  [HttpPost]
  [Route("login")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [AllowAnonymous]
  public async Task<ActionResult> Login([FromBody] UserCredentials credentials)
  {
    if (credentials is null)
    {
      return BadRequest("No user credentials have been supplied.");
    }

    var (isSuccess, message) = await userService.LogInUser(credentials);

    if (isSuccess)
    {
      return Ok(message);
    }

    return BadRequest(message);
  }

  [HttpGet]
  [Route("username")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetUsernameForLoggedUser([FromHeader(Name = "UserId")] Guid userId)
  {
    var (isSuccess, username) = await userService.GetUsernameForLoggedUser(userId);
    if (isSuccess)
    {
      return Ok(username);
    }
    return NotFound();
  }
}
