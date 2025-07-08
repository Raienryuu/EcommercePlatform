using IdentityService.Data;
using IdentityService.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Microsoft.AspNetCore.TestHost;

namespace IdentityService.Tests;

public class AppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    _=builder.ConfigureTestServices(services =>
    {
      _ = services.RemoveAll<ApplicationDbContext>();
      _ = services.RemoveAll<DbContextOptions>();
      _ = services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
      var connection = _dbContainer.GetConnectionString();
      _ = services.AddDbContext<ApplicationDbContext, ApplicationDbContextFake>();
      _ = services.AddSingleton(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(_dbContainer.GetConnectionString())
        .EnableSensitiveDataLogging()
        .Options);

      //services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
      //  options.SignIn.RequireConfirmedAccount = false)
      //  .AddEntityFrameworkStores<ApplicationDbContextFake>();

      // var context = services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
      // _ = context.Database.EnsureCreated();
      // (context as ApplicationDbContextFake)!.FillData();
    });
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
  }
}
