using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductService.Tests.Fakes;
using Testcontainers.MsSql;

namespace ProductService.Tests;

public class AppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _sql = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-GDR1-ubuntu-22.04")
    .Build();

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    _ = builder.ConfigureTestServices(services =>
    {
      _ = services.RemoveAll<ProductDbContext>();
      _ = services.RemoveAll<DbContextOptions<ProductDbContext>>();

      _ = services.AddDbContext<ProductDbContext, ProductDbContextFake>(o =>
        o.UseSqlServer(_sql.GetConnectionString())
      );

      var provider = services.BuildServiceProvider();
      using var scope = provider.CreateAsyncScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
      _ = dbContext.Database.EnsureCreated();
      new ProductDbContextFakeBuilder().FillWithTestData(dbContext);
    });
  }

  public async Task InitializeAsync()
  {
    await _sql.StartAsync().ConfigureAwait(false);
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _sql.DisposeAsync().AsTask();
  }
}
