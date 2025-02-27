using CartService.Services;
using CartService.Tests.Fakes;
using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartService.Tests.Fixtures;

public class CartApp : AppFixture<Program>
{

  protected override void ConfigureServices(IServiceCollection s)
  {
    _ = s.RemoveAll<RedisCartRepository>();
    _ = s.AddSingleton<ICartRepository, FakeCartRepository>();
  }
}
