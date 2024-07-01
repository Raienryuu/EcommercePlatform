using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
	  services.AddDbContext<OrderDbContext, FakeOrderDbContext>();
	});
  }
}