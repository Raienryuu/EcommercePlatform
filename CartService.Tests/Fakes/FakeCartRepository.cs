using CartService.Requests;
using CartService.Services;

namespace CartService.Tests.Fakes
{
  public class FakeCartRepository : ICartRepository
  {

	private Dictionary<Guid, Cart> _products = [];

	public async Task<Guid> UpdateCart(UpdateCart c)
	{
	  var cart = _products.FirstOrDefault(x => x.Key == c.CartGuid);
	  if (cart.Key == Guid.Empty)
	  {
		var cartId = await CreateNewCart(c.Cart);
		return cartId;
	  }

	  foreach (var newProduct in c.Cart.Products)
	  {
		var oldProduct = cart.Value.Products
		  .FirstOrDefault(p => p.Id == newProduct.Id);
		if (oldProduct != null)
		{
		  oldProduct.Amount = newProduct.Amount;
		}
		else
		{
		  cart.Value.Products.Add(newProduct);
		}

		cart.Value.Products = cart.Value.Products
		  .Where(p => p.Amount > 0).ToList();
	  }

	  return cart.Key;
	}

	public Task<Guid> CreateNewCart(Cart c)
	{
	  var newId = MassTransit.NewId.NextSequentialGuid();
	  _products.Add(newId, c);
	  return Task.FromResult(newId);
	}

	public Task DeleteCart(Guid c)
	{
	  _products.Remove(c);
	  return Task.CompletedTask;
	}

	public Task<Cart?> GetCart(Guid c)
	{
	  _products.TryGetValue(c, out Cart? cart);
	  return Task.FromResult(cart);
	}
  }
}