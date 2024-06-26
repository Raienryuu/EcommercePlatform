using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints
{
  public class DeleteCartEndpoint : Endpoint<string>
  {
	private readonly ICartRepository _cartProvider;

	public DeleteCartEndpoint(ICartRepository cartProvider)
	{
	  _cartProvider = cartProvider;
	}
	public override void Configure()
	{
	  Delete("api/cart/deleteItem/{CartId}");
	  AllowAnonymous();
	}

	

	public override async Task HandleAsync(string req, CancellationToken ct)
	{
	  Guid id = Guid.Parse(req);
	  await _cartProvider.DeleteCart(id);
	  await SendAsync(null, cancellation: CancellationToken.None);
	}
  }
}
