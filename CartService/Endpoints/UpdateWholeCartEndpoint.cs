using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class UpdateWholeCartEndpoint(ICartRepository cartRepository)
  : Endpoint<UpdateCart, Guid>
{
  public override void Configure()
  {
    Put("api/cart/updateCart");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateCart req, CancellationToken ct)
  {
    var cartGuid = await cartRepository.UpdateWholeCart(req);
    await SendAsync(cartGuid, cancellation: ct);
  }
}
