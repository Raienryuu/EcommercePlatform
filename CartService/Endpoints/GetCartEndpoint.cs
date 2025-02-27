using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class GetCartEndpoint(ICartRepository cartProvider) : Endpoint<string, Cart?>
{
  public override void Configure()
  {
    Get("api/cart/{cartGuid}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(string req, CancellationToken ct)
  {
    if (!Guid.TryParse(req, out var idAsGuid))
    {
      await SendAsync(null, 400, ct);
    }
    var cart = await cartProvider.GetCart(idAsGuid);
    await SendAsync(cart, cancellation: CancellationToken.None);
  }
}
