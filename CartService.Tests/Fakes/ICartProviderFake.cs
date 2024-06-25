using System.Text.Json;
using CartService.Requests;
using CartService.Services;

public class FakeCartRepository : ICartRepository
{

  private Dictionary<Guid, Cart> _products = [];

  public async Task<Guid> AddNewItem(UpdateCart c)
  {
	var cart = _products.FirstOrDefault(x => x.Key == c.CartGuid);
	if (cart.Key == Guid.Empty)
	{
	  var cartId = await CreateNewCart(c.Cart);
	  return cartId;
	}
	cart.Value.Products.AddRange(c.Cart.Products);
	return cart.Key;
  }

  public async Task<Guid> CreateNewCart(Cart c)
  {
	var newId = MassTransit.NewId.NextSequentialGuid();
	_products.Add(newId, c);
	return newId;
  }
}