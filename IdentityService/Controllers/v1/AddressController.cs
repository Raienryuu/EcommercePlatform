using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers.V1;

[Route("api/v1/addresses")]
[ApiController]
public class AddressController(IAddressesService addresses) : ControllerBase
{
  private readonly IAddressesService _addresses = addresses;

  // GET: api/<AddressController>
  [HttpGet]
  public async Task<ActionResult<List<UserAddress>>> Get([FromHeader(Name = "UserId")] Guid userId)
  {
    return Ok(await _addresses.GetAddressesForUserAsync(userId));
  }

  // GET api/<AddressController>/5
  [HttpGet("{id:int}")]
  [ActionName("GetAddressById")]
  public async Task<ActionResult<UserAddress>> GetAddressById(
    [FromHeader(Name = "UserId")] Guid userId,
    int id
  )
  {
    var result = await _addresses.GetAddressAsync(id);

    if (!result.IsSuccess)
    {
      return Problem(result.ErrorMessage, statusCode: result.StatusCode);
    }

    if (result.Value.UserId != userId)
    {
      return Forbid();
    }

    return Ok(result.Value);
  }

  // POST api/<AddressController>
  [HttpPost]
  public async Task<ActionResult<UserAddress>> Post(
    [FromHeader(Name = "UserId")] Guid userId,
    [FromBody] UserAddress address
  )
  {
    address.UserId = userId;
    var result = await _addresses.AddAddressAsync(address);

    if (!result.IsSuccess)
    {
      return StatusCode(result.StatusCode, result.ErrorMessage);
    }

    return CreatedAtAction(nameof(GetAddressById), new { id = result.Value.Id }, result.Value);
  }

  // PUT api/<AddressController>
  [HttpPut]
  public async Task<ActionResult<UserAddress>> Put(
    [FromHeader(Name = "UserId")] Guid userId,
    [FromBody] UserAddress newAddresss
  )
  {
    var addressInDb = await _addresses.GetAddressAsync(newAddresss.Id);

    if (!addressInDb.IsSuccess)
    {
      return StatusCode(addressInDb.StatusCode, addressInDb.ErrorMessage);
    }

    if (addressInDb.Value.UserId != userId)
    {
      return Forbid();
    }

    var result = await _addresses.EditAddressAsync(newAddresss);

    return result.IsSuccess ? Ok(result.Value) : StatusCode(result.StatusCode, result.ErrorMessage);
  }

  // DELETE api/<AddressController>/5
  [HttpDelete("{id:int}")]
  public async Task<ActionResult> Delete([FromHeader(Name = "UserId")] Guid userId, int id)
  {
    var address = await _addresses.GetAddressAsync(id);

    if (!address.IsSuccess)
    {
      return StatusCode(address.StatusCode, address.ErrorMessage);
    }

    if (address.Value.UserId != userId)
    {
      return Forbid();
    }

    var result = await _addresses.RemoveAddressAsync(id);

    if (!result.IsSuccess)
    {
      return StatusCode(result.StatusCode, result.ErrorMessage);
    }

    return NoContent();
  }
}
