using System.Text.Json;
using CartService.Helpers;
using CartService.Models;
using Common;
using MassTransit;
using System.Net;
using StackExchange.Redis;

namespace CartService.Services;

public class RedisCartRepository(RedisConnectionFactory dbFactory) : ICartRepository
{
  private readonly IDatabase _db = dbFactory.connection.GetDatabase();

  public async Task<ServiceResult<Guid>> CreateNewCart(Cart c)
  {
    var newId = NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);

    return await SetCartValue(newId, c);
  }

  public async Task<ServiceResult> DeleteCart(Guid g)
  {
    var idString = g.ToString();
    return await _db.KeyDeleteAsync(idString)
      ? ServiceResults.Success(HttpStatusCode.OK)
      : ServiceResults.Error("There was a problem deleting the cart", HttpStatusCode.InternalServerError);
  }

  public async Task<ServiceResult<Cart>> GetCart(Guid g)
  {
    var objectJson = await _db.StringGetAsync(g.ToString());
    if (objectJson.IsNullOrEmpty)
    {
      return ServiceResults.Error("Didn't find the specified cart.", HttpStatusCode.NotFound);
    }
    var cartAsString = objectJson.ToString();
    var cart = JsonSerializer.Deserialize<Cart>(cartAsString);

    return ServiceResults.Success(cart!, HttpStatusCode.OK);
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
      ? ServiceResults.Success(key, HttpStatusCode.OK)
      : ServiceResults.Error("Could not create new cart", HttpStatusCode.InternalServerError);
  }
}
