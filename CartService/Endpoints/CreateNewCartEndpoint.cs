using CartService.Requests;
using CartService.Services;
using CartService.Validators;
using FastEndpoints;
using MassTransit;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.Endpoints
{
  public class CreateNewCartEndpoint : Endpoint<Cart>
  {
	private readonly ICartRepository _cartProvider;
	public CreateNewCartEndpoint(ICartRepository cartProvider)
	{
	  _cartProvider = cartProvider;
	}

	public override void Configure()
	{
	  Post("api/cart");
	  AllowAnonymous();
	}

	public override async Task HandleAsync(Cart req, CancellationToken ct)
	{
	  var newId = await _cartProvider.CreateNewCart(req);
	  await SendCreatedAtAsync("api/cart/{guid}", newId, newId, cancellation: CancellationToken.None);
	}
  }
}
