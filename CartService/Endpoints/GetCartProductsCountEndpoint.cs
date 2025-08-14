using CartService.Requests;
using CartService.Services;
using FastEndpoints;

namespace CartService.Endpoints;

public class GetCartProductsCountEndpoint(ICartRepository cartProvider) : Endpoint<GetCartRequest, int>
{
  public override void Configure()
  {
    Get("api/cart/{@cartGuid}/count", static x => new { x.Id });
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetCartRequest request, CancellationToken ct)
  {
    var getResult = await cartProvider.GetCart(request.Id);
    if (!getResult.IsSuccess)
    {
      await SendResultAsync(TypedResults.Problem(getResult.ErrorMessage, statusCode: (int)getResult.StatusCode));
    }
    else
    {
      var distinctProductsCount = getResult.Value.Products.Count;
      await SendOkAsync(distinctProductsCount, ct);
    }
  }
}
