using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using OrderService;

namespace OrderService.Tests;

public class AppFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);

    builder.ConfigureTestServices(services =>
    {
      services.AddMassTransitTestHarness(o =>
      {
      });
    });

  }
}