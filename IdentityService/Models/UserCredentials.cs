namespace IdentityService.Models;

public class UserCredentials : IUserData
{
  public required string Login { get; set; }
  public required string Password { get; set; }
}
