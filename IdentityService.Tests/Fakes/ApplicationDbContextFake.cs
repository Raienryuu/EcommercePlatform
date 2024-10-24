using System.Diagnostics;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Tests.Fakes;

public class ApplicationDbContextFake : ApplicationDbContext
{
  public ApplicationDbContextFake(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public void FillData()
  {
	var userRole = new IdentityRole<Guid>("User")
	{
	  NormalizedName = "USER"
	};

	Roles.Add(userRole);

	var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");
	var user = new IdentityUser<Guid>
	{
	  UserName = "aliCe",
	  NormalizedUserName = "ALICE",
	  Email = "alieCeCe@mail.com",
	  NormalizedEmail = "ALIECECE@MAIL.COM",
	  EmailConfirmed = true,
	  PasswordHash = "AQAAAAIAAYagAAAAENoY1Xiw34/MepI8PP5OOPRdbXcp/3U+2lVNhPKfhdW7HOg1kJGTPsl9usMff5p5Pw==",
	  TwoFactorEnabled = false,
	  LockoutEnabled = true,
	  Id = aliceGuid
	};
	Users.Add(user);
	var address = new UserAddress //Id = 1
	{
	  Address = "Unit St",
	  City = "Sample City",
	  Country = "United States of America",
	  Email = "aliciala@mail.com",
	  FullName = "Alice Testmark",
	  PhoneNumber = "+1324876582",
	  ZIPCode = "16789-968",
	  UserId = aliceGuid
	};
	Addresses.Add(address);
	SaveChanges();
  }
}