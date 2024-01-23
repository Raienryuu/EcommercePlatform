using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.Unit.SampleData
{
    public class SampleUserData
    {
        public static readonly NewUser newUser = new()
        {
            UserName = "user111",
            Password = "passwd",
            Name = "Tom",
            Lastname = "Dodo",
            Email = "dodoto@mailtown.com",
            PhonePrefix = "48",
            PhoneNumber = "823928132",
            Address = "2231 Oliver Street",
            City = "Fort Worth",
            ZIPCode = "76147",
            Country = "United States"
        };

        public static readonly IdentityUser identityUser = new()
        {
            AccessFailedCount = 0,
            ConcurrencyStamp = "7ae4e9ad-36bd-4fa4-af96-d81e96bcb6bf",
            Email = "alieCeCe@mail.com",
            EmailConfirmed = true,
            Id = "1d57dd4e-ae44-458b-9d05-65bbf0998d7f",
            LockoutEnabled = true,
            NormalizedEmail = "ALIECECE@MAIL.COM",
            NormalizedUserName = "ALICE",
            PasswordHash = "AQAAAAIAAYagAAAAENoY1Xiw34/MepI8PP5OOPRdbXcp/3U+2lVNhPKfhdW7HOg1kJGTPsl9usMff5p5Pw==",
            PhoneNumber = "857740321",
            SecurityStamp = "RWFQ2XIUX7FXAVSM4MM6OXJRE4DXP6N5",
            TwoFactorEnabled = false,
            UserName = "aliCe"
        };

        public static readonly UserCredentials userCredentials = new()
        {
            Login = "user111",
            Password = "passwd"
        };
    }
}
