using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class UpdateCartEndpoint(ICartRepository cartProvider) : Endpoint<UpdateCart>
{
  public override void Configure()
  {
    Patch("api/cart/addItem/{cartGuid}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateCart req, CancellationToken ct)
  {
    var cartGuid = await cartProvider.UpdateCart(req);
    await SendAsync(cartGuid, cancellation: CancellationToken.None);
  }
}
