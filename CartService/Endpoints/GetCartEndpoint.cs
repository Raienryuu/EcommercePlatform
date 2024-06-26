using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints
{
  public class GetCartEndpoint : Endpoint<string, Cart?>
  {
	private readonly ICartRepository _cartProvider;

	public GetCartEndpoint(ICartRepository cartProvider)
	{
	  _cartProvider = cartProvider;
	}
	public override void Configure()
	{
	  Get("api/cart/{cartGuid}");
	  AllowAnonymous();
	}

	public override async Task HandleAsync(string req, CancellationToken ct)
	{
	  var idAsGuid = Guid.Parse(req);
	  var cart = await _cartProvider.GetCart(idAsGuid);
	  await SendAsync(cart, cancellation: CancellationToken.None);
	}
  }
}
