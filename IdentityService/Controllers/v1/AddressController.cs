using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
  [HttpGet("{id}")]
  [ActionName("GetAddressById")]
  public async Task<ActionResult<UserAddress>> GetAddressById(
    [FromHeader(Name = "UserId")] Guid userId,
    int id
  )
  {
    var address = await _addresses.GetAddressAsync(id);
    if (address?.UserId != userId)
    {
      return Forbid();
    }

    if (address is null)
      return NotFound();
    return Ok(address);
  }

  // POST api/<AddressController>
  [HttpPost]
  public async Task<ActionResult<UserAddress>> Post(
    [FromHeader(Name = "UserId")] Guid userId,
    [FromBody] UserAddress address
  )
  {
    address.UserId = userId;
    var addedAddress = await _addresses.AddAddressAsync(address);
    return CreatedAtAction(nameof(GetAddressById), new { id = addedAddress.Id }, addedAddress);
  }

  // PUT api/<AddressController>
  [HttpPut]
  public async Task<ActionResult<UserAddress>> Put(
    [FromHeader(Name = "UserId")] Guid userId,
    [FromBody] UserAddress newAddresss
  )
  {
    var addressInDb = await _addresses.GetAddressAsync(newAddresss.Id);
    if (addressInDb?.UserId != userId)
    {
      return Forbid();
    }
    if (addressInDb is null)
      return NotFound();
    return Ok(await _addresses.EditAddressAsync(newAddresss));
  }

  // DELETE api/<AddressController>/5
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete([FromHeader(Name = "UserId")] Guid userId, int id)
  {
    var address = await _addresses.GetAddressAsync(id);
    if (address?.UserId != userId)
    {
      return Forbid();
    }
    await _addresses.RemoveAddressAsync(id);
    return NoContent();
  }
}
