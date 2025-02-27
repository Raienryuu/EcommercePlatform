using System.Text.Json;
using CartService.Helpers;
using CartService.Requests;
using MassTransit;
using StackExchange.Redis;

namespace CartService.Services;

public class RedisCartRepository(RedisConnectionFactory dbFactory) : ICartRepository
{
  private readonly IDatabase _db = dbFactory.connection.GetDatabase();

  public async Task<Guid> UpdateCart(UpdateCart c)
  {
    var cartJson = await _db.StringGetAsync(c.CartGuid.ToString());
    var cartAsString = cartJson.ToString();
    var cart = JsonSerializer.Deserialize<Cart>(cartAsString);

    if (cart is null)
    {
      return await CreateNewCart(c.Cart);
    }

    cart.Products.AddRange(c.Cart.Products);
    cart = CartHelper.MergeCart(cart);

    _ = await _db.StringSetAsync(c.CartGuid.ToString(), JsonSerializer.Serialize(cart));
    return c.CartGuid;
  }

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

  public async Task<Guid> UpdateWholeCart(UpdateCart c)
  {
    c.Cart = CartHelper.MergeCart(c.Cart);
    var objectJson = await _db.StringGetAsync(c.CartGuid.ToString());
    return objectJson.IsNullOrEmpty
      ? await CreateNewCart(c.Cart)
      : await _db.StringSetAsync(c.CartGuid.ToString(), JsonSerializer.Serialize(c.Cart))
          ? c.CartGuid
          : throw new RedisCommandException("Could not create new cart");
  }
}
