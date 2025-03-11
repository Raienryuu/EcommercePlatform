using CartService.Mappers;
using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class UpdateCartEndpoint(ICartRepository cartRepository) : Endpoint<UpdateCartRequest, Guid>
{
  public override void Configure()
  {
    Put("api/cart/{@cartGuid}", static x => new { x.Id });
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateCartRequest request, CancellationToken ct)
  {
    var newCart = request.ToCart();
    var cartGuid = await cartRepository.UpdateCart(request.Id, newCart);
    await SendAsync(cartGuid, cancellation: ct);
  }
}
