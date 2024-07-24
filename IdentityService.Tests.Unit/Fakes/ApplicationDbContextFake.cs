using System.Diagnostics;
using IdentityService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Tests.Unit.Fakes;

public class ApplicationDbContextFake : ApplicationDbContext
{
    public ApplicationDbContextFake(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public void FillData()
    {
        var userRole = new IdentityRole("User")
        {
            NormalizedName = "USER"
        };

        Roles.Add(userRole);

        var user = new IdentityUser
        {
            UserName = "aliCe",
            NormalizedUserName = "ALICE",
            Email = "alieCeCe@mail.com",
            NormalizedEmail = "LIECECE@MAIL.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAENoY1Xiw34/MepI8PP5OOPRdbXcp/3U+2lVNhPKfhdW7HOg1kJGTPsl9usMff5p5Pw==",
            TwoFactorEnabled = false,
            LockoutEnabled = true
        };
        Users.Add(user);
        SaveChanges();
    }
}