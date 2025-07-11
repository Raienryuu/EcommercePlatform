namespace IdentityService.Models
{
  public class NewUser
  {
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required UserAddress Address { get; set; }
  }
}
