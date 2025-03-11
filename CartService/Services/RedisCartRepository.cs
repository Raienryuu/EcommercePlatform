using System.Text.Json;
using CartService.Helpers;
using CartService.Models;
using MassTransit;
using StackExchange.Redis;

namespace CartService.Services;

public class RedisCartRepository(RedisConnectionFactory dbFactory) : ICartRepository
{
  private readonly IDatabase _db = dbFactory.connection.GetDatabase();

  public async Task<Guid> CreateNewCart(Cart c)
  {
    var newId = NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);

    return await _db.StringSetAsync(newId.ToString(), JsonSerializer.Serialize(c))
      ? newId
      : throw new RedisCommandException("Could not create new cart");
  }

  public async Task DeleteCart(Guid g)
  {
    var idString = g.ToString();
    _ = await _db.KeyDeleteAsync(idString);
  }

  public async Task<Cart?> GetCart(Guid g)
  {
    var objectJson = await _db.StringGetAsync(g.ToString());
    var cartAsString = objectJson.ToString();

    return objectJson.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart?>(cartAsString);
  }

  public async Task<Guid> UpdateCart(Guid id, Cart c)
  {
    c = CartHelper.MergeCart(c);
    var objectJson = await _db.StringGetAsync(id.ToString());
    return objectJson.IsNullOrEmpty ? await CreateNewCart(c)
      : await _db.StringSetAsync(id.ToString(), JsonSerializer.Serialize(c)) ? id
      : throw new RedisCommandException("Could not create new cart");
  }
}
