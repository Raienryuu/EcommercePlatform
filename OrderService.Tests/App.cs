using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService;
using OrderService.Tests.Fakes;

namespace OrderService.Tests;

public class App : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
	base.ConfigureWebHost(builder);

	builder.ConfigureTestServices(services =>
	{
	  services.AddMassTransitTestHarness(o =>
	  {
	  });

	  services.RemoveAll<OrderDbContext>();
	  var dbcontextInstance = new FakeOrderDbContextBuilder().CreateDbContext();
	  services.AddSingleton<OrderDbContext>(dbcontextInstance);
	});
  }

}