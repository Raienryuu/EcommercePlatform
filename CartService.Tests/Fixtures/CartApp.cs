using FastEndpoints;
using FastEndpoints.Testing;
using CartService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CartService.Services;


namespace CartService.Tests.Fixtures
{
  public class CartApp : AppFixture<Program>
  {

	protected override void ConfigureServices(IServiceCollection s)
	{
	  s.RemoveAll<RedisCartRepository>();
	  s.AddScoped<ICartRepository, FakeCartRepository>();
	}

  }
}
