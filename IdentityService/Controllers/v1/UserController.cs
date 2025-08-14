using System.Text.Json;
using IdentityService.Models;
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
    var result = await userService.RegisterNewUser(registrationData);

    return result.IsSuccess ? Ok() : Problem(result.ErrorMessage, statusCode: (int)result.StatusCode);
  }

  [HttpPost]
  [Route("login")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [AllowAnonymous]
  public async Task<ActionResult> Login([FromBody] UserCredentials credentials)
  {
    var result = await userService.LogInUser(credentials);

    return result.IsSuccess ? Ok(result.Value) : Problem(result.ErrorMessage, statusCode: (int)result.StatusCode);
  }

  [HttpGet]
  [Route("username")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetUsernameForLoggedUser([FromHeader(Name = "UserId")] Guid userId)
  {
    var result = await userService.GetUsernameForLoggedUser(userId);
    return result.IsSuccess
      ? Ok(JsonSerializer.Serialize(result.Value))
      : Problem(result.ErrorMessage, statusCode: (int)result.StatusCode);
  }
}
