using CartService.Requests;
using CartService.Services;
using CartService.Validators;
using FastEndpoints;
using MassTransit;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.Endpoints
{
  public class CreateNewCartEndpoint : Endpoint<Cart, Guid>
  {
	private readonly ICartRepository _cartProvider;
	public CreateNewCartEndpoint(ICartRepository cartProvider)
	{
	  _cartProvider = cartProvider;
	}

	public override void Configure()
	{
	  Post("api/cart/create");
	  AllowAnonymous();
	}

	public override async Task HandleAsync(Cart req, CancellationToken ct)
	{
	  var newId = _cartProvider.CreateNewCart(req);
	  await SendAsync(newId, cancellation: ct);
	}
  }
}
