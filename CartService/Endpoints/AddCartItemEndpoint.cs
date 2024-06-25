using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints
{
  public class AddCartItemEndpoint : Endpoint<UpdateCart>
  {
	private readonly ICartRepository _cartProvider;

	public AddCartItemEndpoint(ICartRepository cartProvider)
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
	  var cartGuid = await _cartProvider.AddNewItem(req);
	  await SendAsync(cartGuid, cancellation: CancellationToken.None);
	}
  }
}
