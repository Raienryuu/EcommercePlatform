using System.Net;
using CartService.Helpers;
using CartService.Models;
using CartService.Services;
using Common;

namespace CartService.Tests.Fakes;

public class FakeCartRepository : ICartRepository
{
  private readonly Dictionary<Guid, Cart> _products = [];

  public Task<ServiceResult<Guid>> CreateNewCart(Cart c)
  {
    var newId = MassTransit.NewId.NextSequentialGuid();
    c = CartHelper.MergeCart(c);
    _products.Add(newId, c);
    return Task.FromResult<ServiceResult<Guid>>(ServiceResults.Success(newId, HttpStatusCode.OK));
  }

  public Task<ServiceResult> DeleteCart(Guid g)
  {
    _ = _products.Remove(g);
    return Task.FromResult<ServiceResult>(ServiceResults.Success(HttpStatusCode.OK));
  }

  public Task<ServiceResult<Cart>> GetCart(Guid g)
  {
    var retrievalSuccessful = _products.TryGetValue(g, out var cart);
    return retrievalSuccessful
      ? Task.FromResult<ServiceResult<Cart>>(ServiceResults.Success<Cart>(cart!, HttpStatusCode.OK))
      : Task.FromResult<ServiceResult<Cart>>(ServiceResults.Error<Cart>("Didn't found the cart", HttpStatusCode.NotFound));
  }

  public Task<ServiceResult<Guid>> UpdateCart(Guid id, Cart c)
  {
    c = CartHelper.MergeCart(c);
    if (!_products.TryGetValue(id, out _))
    {
      return CreateNewCart(c);
    }

    _products[id] = c;
    return Task.FromResult<ServiceResult<Guid>>(ServiceResults.Success(id, HttpStatusCode.OK));
  }
}
