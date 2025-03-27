using CartService.Models;
using CartService.Requests;
using FastEndpoints;
using FluentValidation;

namespace CartService.Validators;

public class UpdateCartValidator : Validator<UpdateCartRequest>
{
  public UpdateCartValidator()
  {
    _ = RuleFor(static c => c.Products).NotEmpty();
    RuleForEach(static c => c.Products).SetValidator(new ProductValidator());
  }
}

public class CreateCartValidator : Validator<CreateNewCartRequest>
{
  public CreateCartValidator()
  {
    _ = RuleFor(static c => c.Products).NotEmpty();
    RuleForEach(static c => c.Products).SetValidator(new ProductValidator());
  }
}

public class ProductValidator : Validator<Product>
{
  public ProductValidator()
  {
    _ = RuleFor(static x => x.Amount).GreaterThan(0);
  }
}
