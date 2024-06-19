using CartService.Requests;
using MassTransit;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.Services
{
  public class RedisCartRepository : ICartRepository
  {
	private readonly IDatabase _db;
	public RedisCartRepository(RedisConnectionFactory dbFactory)
	{
	  _db = dbFactory.connection.GetDatabase();
	}
	public Guid CreateNewCart(Cart c)
	{
	  var newId = NewId.NextSequentialGuid();
	  _db.StringSet(newId.ToString(), JsonSerializer.Serialize(c));
	  return newId;
	}
  }
}
