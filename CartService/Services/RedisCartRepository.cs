using System.Text.Json;
using CartService.Helpers;
using CartService.Models;
using MassTransit;
using StackExchange.Redis;

namespace CartService.Services;

public class RedisCartRepository(RedisConnectionFactory dbFactory, ILogger<RedisCartRepository> logger)
  : ICartRepository
{
  private readonly IDatabase _db = dbFactory.connection.GetDatabase();

  public Task<Guid> CreateNewCart(Cart c)
  {
    var newId = NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);

    return SetCartValue(newId, c);
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
    logger.LogCritical("This is new cart that will be saved {c}", objectJson);
    return objectJson.IsNullOrEmpty ? await CreateNewCart(c) : await SetCartValue(id, c);
  }

  private async Task<Guid> SetCartValue(Guid key, Cart value)
  {
    return await _db.StringSetAsync(key.ToString(), JsonSerializer.Serialize(value))
      ? key
      : throw new RedisCommandException("Could not create new cart");
  }
}
