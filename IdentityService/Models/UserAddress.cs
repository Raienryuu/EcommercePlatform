using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityService.Models;

public class UserAddress
{
  [Key]
  public int Id { get; set; }
  public required string FullName { get; set; }
  public required string Email { get; set; }
  public required string PhoneNumber { get; set; }
  public required string Address { get; set; }
  public required string City { get; set; }
  public required string ZIPCode { get; set; }
  public required string Country { get; set; }

  public IdentityUser<Guid>? User { get; set; }

  [ForeignKey("User")]
  public Guid? UserId { get; set; }

  public static UserAddress CreateFrom(NewUser user, IdentityUser<Guid> account)
  {
    UserAddress userAddress = new()
    {
      FullName = user.Address.FullName,
      Email = user.Address.Email,
      PhoneNumber = user.Address.PhoneNumber,
      Address = user.Address.Address,
      City = user.Address.City,
      ZIPCode = user.Address.ZIPCode,
      Country = user.Address.Country,
      UserId = account.Id,
    };
    return userAddress;
  }
}
