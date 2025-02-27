using CartService.Endpoints;
using CartService.Requests;
using CartService.Tests.Fakes;
using CartService.Tests.Fixtures;
using FastEndpoints;
using FastEndpoints.Testing;
using MassTransit;

namespace CartService.Tests;

public class CartServiceTests(CartApp app) : TestBase<CartApp>
{
  [Fact]
  public async Task CreateNewCart_ValidCart_NewCardId()
  {
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var cut = Factory.Create<CreateNewCartEndpoint>(new FakeCartRepository());

    await cut.HandleAsync(cart, TestContext.Current.CancellationToken);
    var res = (Guid)cut.Response!;

    _ = Assert.IsType<Guid>(res);
  }

  [Fact]
  public async Task CreateNewCart_EmptyCart_ErrorCode()
  {
    var cart = new Cart
    {
      Products = []
    };

    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, ErrorResponse>(cart);

    Assert.Contains("must not be empty",
      res.Errors.ElementAt(0).Value.ElementAt(0));
  }

  [Fact]
  public async Task CreateNewCart_AmountIsZeroOrLess_ErrorCode()
  {
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 0
        }
      ]
    };

    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, ErrorResponse>(cart);

    Assert.Contains("greater than '0'",
      res.Errors.ElementAt(0).Value.ElementAt(0));
  }

  [Fact]
  public async Task UpdateCart_ValidItem_CartId()
  {
    const int PRODUCTS_COUNT_AFTER_UPDATE = 2;
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);
    var updateCart = new UpdateCart
    {
      CartGuid = res,
      Cart = new Cart
      {
        Products =
        [
          new()
          {
            Id = Guid.Parse("11187665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
            Amount = 2
          }
        ]
      }
    };

    _ = await app.Client.PATCHAsync<UpdateCartEndpoint, UpdateCart, Guid>(updateCart);

    var (_, finalCart)
      = await app.Client.GETAsync<GetCartEndpoint, string, Cart?>(
        res.ToString());
    Assert.Equal(PRODUCTS_COUNT_AFTER_UPDATE, finalCart?.Products.Count);
  }

  [Fact]
  public async Task DeleteCart_ExistingCartId_OKResponse()
  {
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);

    var httpRes = await app.Client
      .DELETEAsync<DeleteCartEndpoint, string>(res.ToString());

    Assert.True(httpRes.IsSuccessStatusCode);
  }

  [Fact]
  public async Task DeleteCart_NonExistentCartId_OKResponse()
  {
    var guid = NewId.NextGuid().ToString();

    var httpRes = await app.Client
      .DELETEAsync<DeleteCartEndpoint, string>(guid);

    Assert.True(httpRes.IsSuccessStatusCode);
  }

  [Fact]
  public async Task GetCart_ExistingCardId_Cart()
  {
    var newCart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(newCart);
    var (_, cart) = await app.Client
      .GETAsync<GetCartEndpoint, string, Cart?>(res.ToString());

    Assert.True(cart?.Products.Count > 0);
  }

  [Fact]
  public async Task GetCart_NonExistentCardId_NullCart()
  {
    var idString = Guid.NewGuid().ToString();

    var (_, cart) = await app.Client
      .GETAsync<GetCartEndpoint, string, Cart?>(idString);

    Assert.Null(cart);
  }

  [Fact]
  public async Task CreateNewCart_CartWithDuplicateProducts_CartWithMergedProduct()
  {
    const int PRODUCTS_AFTER_MERGING = 1;
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        },
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 8
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);

    var (_, finalCart)
      = await app.Client.GETAsync<GetCartEndpoint, string, Cart?>(
        res.ToString());
    Assert.Equal(PRODUCTS_AFTER_MERGING, finalCart?.Products.Count);
  }
  [Fact]
  public async Task UpdateCart_ValidItem_CartWithMergedProducts()
  {
    const int PRODUCTS_COUNT_AFTER_UPDATE = 1;
    const int PRODUCT_QUANTITY_AFTER_UPDATE = 7;
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);
    var updateCart = new UpdateCart
    {
      CartGuid = res,
      Cart = new Cart
      {
        Products =
        [
          new()
          {
            Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
            Amount = 2
          }
        ]
      }
    };

    _ = await app.Client.PATCHAsync<UpdateCartEndpoint, UpdateCart, Guid>(updateCart);

    var (_, finalCart)
      = await app.Client.GETAsync<GetCartEndpoint, string, Cart?>(
        res.ToString());

    Assert.Equal(PRODUCTS_COUNT_AFTER_UPDATE, finalCart?.Products.Count);
    Assert.Equal(PRODUCT_QUANTITY_AFTER_UPDATE, finalCart?.Products.First().Amount);
  }

  //test that switches out whole cart to new values
  [Fact]
  public async Task UpdateWholeCart_ValidItem_CartWithMergedProducts()
  {
    const int PRODUCTS_COUNT_AFTER_UPDATE = 1;
    var expectedGuid = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0");
    var cart = new Cart
    {
      Products =
      [
        new()
        {
          Id = Guid.Parse("92d87665-97a9-4200-a354-f1c2cbcb63e0").ToString(),
          Amount = 5
        }
      ]
    };
    var (_, res) = await app.Client
      .POSTAsync<CreateNewCartEndpoint, Cart, Guid>(cart);
    var updateCart = new UpdateCart
    {
      CartGuid = res,
      Cart = new Cart
      {
        Products =
        [
          new()
          {
            Id = expectedGuid.ToString(),
            Amount = PRODUCTS_COUNT_AFTER_UPDATE
          }
        ]
      }
    };

    _ = await app.Client.PUTAsync<UpdateWholeCartEndpoint, UpdateCart, Guid>(updateCart);

    var (_, finalCart) = await app.Client.GETAsync<GetCartEndpoint, string, Cart?>(
        res.ToString());

    Assert.Equal(PRODUCTS_COUNT_AFTER_UPDATE, finalCart?.Products.Count);
    Assert.Equal(expectedGuid.ToString(), finalCart?.Products.First().Id);
  }
}
