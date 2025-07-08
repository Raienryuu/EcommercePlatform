using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class DeleteCartEndpoint(ICartRepository cartProvider) : Endpoint<DeleteCartRequest>
{
  public override void Configure()
  {
    Delete("api/cart/{@cartGuid}", static x => new { x.Id });
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteCartRequest request, CancellationToken ct)
  {
    var result = await cartProvider.DeleteCart(request.Id);
    if (result.IsSuccess)
    {
      await SendNoContentAsync(ct);
    }
    else
    {
      await SendResultAsync(TypedResults.Problem(result.ErrorMessage, statusCode: result.StatusCode));
    }
  }
}
