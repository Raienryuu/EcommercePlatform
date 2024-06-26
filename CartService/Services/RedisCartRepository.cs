using CartService.Requests;
using MassTransit;
using StackExchange.Redis;
using System.Text;
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

	public async Task<Guid> AddNewItem(UpdateCart c)
	{
	  var cartJson = await _db.StringGetAsync(c.CartGuid.ToString());

	  if (cartJson.IsNullOrEmpty)
	  {
		var newCartId = await CreateNewCart(c.Cart);
		return newCartId;
	  }

	  var cart = JsonSerializer.Deserialize<Cart>(cartJson!);
	  cart!.Products.AddRange(c.Cart.Products);

	  await _db.StringSetAsync(c.CartGuid.ToString(), JsonSerializer.Serialize(c));
	  return c.CartGuid;
	}

	public async Task<Guid> CreateNewCart(Cart c)
	{
	  var newId = NewId.NextSequentialGuid();
	  await _db.StringSetAsync(newId.ToString(), JsonSerializer.Serialize(c));
	  return newId;
	}

	public async Task DeleteCart(Guid c)
	{
	  var idString = c.ToString();
	  await _db.KeyDeleteAsync(idString);
	}
  }
}
