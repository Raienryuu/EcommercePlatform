using System.Text.Json;
using CartService.Helpers;
using CartService.Models;
using Common;
using MassTransit;
using StackExchange.Redis;

namespace CartService.Services;

public class RedisCartRepository(RedisConnectionFactory dbFactory) : ICartRepository
{
  private readonly IDatabase _db = dbFactory.connection.GetDatabase();

  public Task<ServiceResult<Guid>> CreateNewCart(Cart c)
  {
    var newId = NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);

    return SetCartValue(newId, c);
  }

  public async Task<ServiceResult> DeleteCart(Guid g)
  {
    var idString = g.ToString();
    return await _db.KeyDeleteAsync(idString)
      ? ServiceResults.Success(200)
      : ServiceResults.Error("There was a problem deleting the cart", 500);
  }

  public async Task<ServiceResult<Cart>> GetCart(Guid g)
  {
    var objectJson = await _db.StringGetAsync(g.ToString());
    if (objectJson.IsNullOrEmpty)
    {
      ServiceResults.Error<Cart>("Didn't find the specified cart.", 404);
    }
    var cartAsString = objectJson.ToString();
    var cart = JsonSerializer.Deserialize<Cart>(cartAsString);

    return ServiceResults.Success(cart!, 200);
  }

  public async Task<ServiceResult<Guid>> UpdateCart(Guid id, Cart c)
  {
    c = CartHelper.MergeCart(c);
    var objectJson = await _db.StringGetAsync(id.ToString());

    if (objectJson.IsNullOrEmpty)
    {
      return await CreateNewCart(c);
    }

    return await SetCartValue(id, c);
  }

  private async Task<ServiceResult<Guid>> SetCartValue(Guid key, Cart value)
  {
    return await _db.StringSetAsync(key.ToString(), JsonSerializer.Serialize(value))
      ? ServiceResults.Success(key, 200)
      : ServiceResults.Error<Guid>("Could not create new cart", 500);
  }
}
