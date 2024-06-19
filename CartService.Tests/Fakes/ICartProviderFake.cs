using System.Text.Json;
using CartService.Requests;
using CartService.Services;

public class FakeCartProvider : ICartRepository
  {

    private Dictionary<Guid, Cart> _products = [];
	public Guid CreateNewCart(Cart c)
	{
	  var newId = MassTransit.NewId.NextSequentialGuid();
	  _products.Add(newId, c);
	  return newId;
	}
  }