using FluentValidation.TestHelper;
using OrderService.Endpoints.Requests;
using OrderService.Models;
using OrderService.Validators;
using Xunit;

namespace OrderService.Tests.Unit;

public class OrderValidatorTests
{
  private readonly OrderValidator _validator = new();

  [Fact]
  public void OrderValidator_ProductsIsEmpty_ShouldHaveValidationError()
  {
    var model = new CreateOrderRequest
    {
      Products = [],
      CurrencyISO = "USD"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Products);
  }

  [Fact]
  public void OrderValidator_ProductQuantityIsZero_ShouldHaveValidationError()
  {
    var model = new CreateOrderRequest
    {
      Products =
        [
            new OrderProduct { Id = 1, ProductId = Guid.NewGuid(), Quantity = 0, Price = 100 },
                new OrderProduct { Id = 2, ProductId = Guid.NewGuid(), Quantity = 2, Price = 200 }
        ],
      CurrencyISO = "USD"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveAnyValidationError();
    result.ShouldHaveValidationErrorFor("Products[0].Quantity");
  }

  [Fact]
  public void OrderValidator_ProductsValid_ShouldNotHaveAnyValidationErrors()
  {
    var model = new CreateOrderRequest
    {
      Products =
        [
          new OrderProduct { Id = 1, ProductId = Guid.NewGuid(), Quantity = 1, Price = 100 },
          new OrderProduct { Id = 2, ProductId = Guid.NewGuid(), Quantity = 2, Price = 200 }
        ],
      CurrencyISO = "USD"
    };
    var result = _validator.TestValidate(model);
    result.ShouldNotHaveAnyValidationErrors();
  }
}