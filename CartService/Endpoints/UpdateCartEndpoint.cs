using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints
{
  public class UpdateCartEndpoint : Endpoint<UpdateCart>
  {
	private readonly ICartRepository _cartProvider;

	public UpdateCartEndpoint(ICartRepository cartProvider)
	{
	  _cartProvider = cartProvider;
	}

	public override void Configure()
	{
	  Patch("api/cart/addItem/{cartGuid}");
	  AllowAnonymous();
	}

	public override async Task HandleAsync(UpdateCart req, CancellationToken ct)
	{
	  var cartGuid = await _cartProvider.UpdateCart(req);
	  await SendAsync(cartGuid, cancellation: CancellationToken.None);
	}
  }
}
