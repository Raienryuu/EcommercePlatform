using CartService.Helpers;
using CartService.Requests;
using CartService.Services;

namespace CartService.Tests.Fakes;

public class FakeCartRepository : ICartRepository
{

  private readonly Dictionary<Guid, Cart> _products = [];

  public Task<Guid> UpdateCart(UpdateCart c)
  {
    var cart = _products.FirstOrDefault(x => x.Key == c.CartGuid);
    if (cart.Key == Guid.Empty)
    {
      return CreateNewCart(c.Cart);
    }
    cart.Value.Products.AddRange(c.Cart.Products);

    var newCart = CartHelper.MergeCart(cart.Value);
    _products[c.CartGuid] = newCart;
    return Task.FromResult(cart.Key);
  }

  public Task<Guid> CreateNewCart(Cart c)
  {
    var newId = MassTransit.NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);
    _products.Add(newId, c);
    return Task.FromResult(newId);
  }

  public Task DeleteCart(Guid g)
  {
    _ = _products.Remove(g);
    return Task.CompletedTask;
  }

  public Task<Cart?> GetCart(Guid g)
  {
    _ = _products.TryGetValue(g, out var cart);
    return Task.FromResult(cart);
  }

  public Task<Guid> UpdateWholeCart(UpdateCart c)
  {
    c.Cart = CartHelper.MergeCart(c.Cart);
    if (!_products.TryGetValue(c.CartGuid, out _))
    {
      return CreateNewCart(c.Cart);
    }

    _products[c.CartGuid] = c.Cart;
    return Task.FromResult(c.CartGuid);
  }
}
