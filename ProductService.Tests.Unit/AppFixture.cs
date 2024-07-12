using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ProductService;
using ProductServiceTests.Fakes;
using Testcontainers.MsSql;

namespace ProductServiceTests;

public class AppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _sql = new MsSqlBuilder()
  .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-GDR1-ubuntu-22.04")
  .Build();

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
	base.ConfigureWebHost(builder);
	builder.ConfigureTestServices(services =>
	{
	  services.RemoveAll<ProductDbContext>();
	  services.AddSingleton(new ProductDbContextFakeBuilder().BuildFromEmptyWithData(_sql.GetConnectionString()));
	});
  }

  public async Task InitializeAsync()
  {
	await _sql.StartAsync();
  }
  async Task IAsyncLifetime.DisposeAsync()
  {
	await _sql.DisposeAsync().AsTask();
  }
}
