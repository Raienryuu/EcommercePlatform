using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityService.Models;
public class UserAddress : IUserData
{
  [Key]
  public int Id { get; set; }
  public required string Name { get; set; }
  public required string Lastname { get; set; }
  public required string Email { get; set; }
  public required string PhonePrefix { get; set; }
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
	  Name = user.Name,
	  Lastname = user.Lastname,
	  Email = user.Email,
	  PhonePrefix = user.PhonePrefix,
	  PhoneNumber = user.PhoneNumber,
	  Address = user.Address,
	  City = user.City,
	  ZIPCode = user.ZIPCode,
	  Country = user.Country,
	  UserId = account.Id
	};
	return userAddress;
  }

  private static Guid GetGuidFromString(string value)
  {
	var isSuccess = Guid.TryParse(value, out var guid);
	if (!isSuccess)
	{
	  throw new ArgumentException("String value should contain valid Guid");
	}
	return guid;
  }

}
