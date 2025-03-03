using CartService.Helpers;
using CartService.Models;
using CartService.Services;

namespace CartService.Tests.Fakes;

public class FakeCartRepository : ICartRepository
{
  private readonly Dictionary<Guid, Cart> _products = [];

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

  public Task<Guid> UpdateCart(Guid id, Cart c)
  {
    c = CartHelper.MergeCart(c);
    if (!_products.TryGetValue(id, out _))
    {
      return CreateNewCart(c);
    }

    _products[id] = c;
    return Task.FromResult(id);
  }
}
