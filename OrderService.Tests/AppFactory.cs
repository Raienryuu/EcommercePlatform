using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService.Services;
using OrderService.Tests.Fakes;
using Testcontainers.MsSql;

namespace OrderService.Tests;

public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _sql = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-GDR1-ubuntu-22.04")
    .Build();

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);

    builder.ConfigureTestServices(services =>
    {
      services.AddMassTransitTestHarness(o => { });

      services.RemoveAll<OrderDbContext>();
      services.AddDbContext<OrderDbContext, FakeOrderDbContext>();
      services.AddSingleton<DbContextOptions>(
        new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(_sql.GetConnectionString()).Options
      );

      services.AddScoped<IStripePaymentService, FakeStripePaymentService>();

      var provider = services.BuildServiceProvider();
      FillWithFakeData(provider);
    });
  }

  private static void FillWithFakeData(ServiceProvider provider)
  {
    using var scope = provider.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    context.Database.EnsureCreated();
    FakeOrderDataInserter.FillData(context);
  }

  async Task IAsyncLifetime.InitializeAsync()
  {
    await _sql.StartAsync();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _sql.DisposeAsync().AsTask();
  }
}
