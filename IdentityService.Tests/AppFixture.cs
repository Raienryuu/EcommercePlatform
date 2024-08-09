using IdentityService.Data;
using IdentityService.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests
{
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
      builder.ConfigureTestServices(services =>
      {
        services.RemoveAll<ApplicationDbContext>();
        services.RemoveAll<DbContextOptions>();
        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
        var connection = _dbContainer.GetConnectionString();
        services.AddDbContext<ApplicationDbContext, ApplicationDbContextFake>();
        services.AddSingleton<DbContextOptions<ApplicationDbContext>>(new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseSqlServer(_dbContainer.GetConnectionString())
          .EnableSensitiveDataLogging()
          .Options);

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
          options.SignIn.RequireConfirmedAccount = false)
          .AddEntityFrameworkStores<ApplicationDbContextFake>();

        var context = services.BuildServiceProvider().GetService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        (context as ApplicationDbContextFake).FillData();
      });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
      await _dbContainer.DisposeAsync();
    }
  }
}
