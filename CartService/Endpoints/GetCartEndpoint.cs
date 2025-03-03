using CartService.Models;
using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class GetCartEndpoint(ICartRepository cartProvider) : Endpoint<GetCartRequest, Cart?>
{
  public override void Configure()
  {
    Get("api/cart/{@cartGuid}", static x => new { x.Id });
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetCartRequest request, CancellationToken ct)
  {
    var cart = await cartProvider.GetCart(request.Id);
    await SendAsync(cart, cancellation: ct);
  }
}
