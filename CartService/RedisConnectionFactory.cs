using CartService.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CartService
{
  public class RedisConnectionFactory
  {
	public ConnectionMultiplexer connection;

	public RedisConnectionFactory(IOptions<RedisOptions> config)
	{
	  connection = ConnectionMultiplexer.Connect(config.Value.ConnectionString);
	}
  }
}
