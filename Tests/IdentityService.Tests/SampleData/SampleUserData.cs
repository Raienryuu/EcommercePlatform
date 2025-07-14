using Docker.DotNet.Models;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.SampleData
{
  public static class SampleUserData
  {
    public static readonly NewUser NewUser = new()
    {
      UserName = "user111",
      Password = "passwd",
      Address = new UserAddress
      {
        FullName = "Tom Dodo",
        Email = "dodoto@mailtown.com",
        PhoneNumber = "+48823928132",
        Address = "2231 Oliver Street",
        City = "Fort Worth",
        ZIPCode = "76147",
        Country = "United States",
      },
    };
    public static NewUser LoginUser => new()
    {
      UserName = "loginUser",
      Password = "passwd",
      Address = new UserAddress
      {
        FullName = "Alex Dodo",
        Email = "newMail@mailing.com",
        PhoneNumber = "+48755392031",
        Address = "2232 Oliver Street",
        City = "Fort Worth",
        ZIPCode = "76147",
        Country = "United States",
      },
    };
    public static IdentityUser IdentityUser => new()
    {
      AccessFailedCount = 0,
      ConcurrencyStamp = "7ae4e9ad-36bd-4fa4-af96-d81e96bcb6bf",
      Email = "alieCeCe@mail.com",
      EmailConfirmed = true,
      Id = "1d57dd4e-ae44-458b-9d05-65bbf0998d7f",
      LockoutEnabled = true,
      NormalizedEmail = "ALIECECE@MAIL.COM",
      NormalizedUserName = "ALICE",
      PasswordHash = "AQAAAAIAAYagAAAAEEY+AZyfLRepA2qBmTlAki2N/NC2cCAiKHoxmOgzKpcTMz6VuIG2CdFTfjmwlI8MRw==",
      PhoneNumber = "857740321",
      SecurityStamp = "RWFQ2XIUX7FXAVSM4MM6OXJRE4DXP6N5",
      TwoFactorEnabled = false,
      UserName = "aliCe",
    };

    public static UserCredentials UserCredentials => new()
    {
      Login = "loginUser",
      Password = "passwd",
    };

    public static UserAddress SampleAddress => new()
    {
      Address = "Willow St",
      City = "Chicago",
      Country = "United States of America",
      Email = "toomfoolery@mail.com",
      FullName = "Thomas Fool",
      PhoneNumber = "+132132412312",
      ZIPCode = "1423-1234",
    };
  }
}
