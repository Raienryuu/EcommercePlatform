using CartService.Requests;
using FastEndpoints;
using FluentValidation;

namespace CartService.Validators;

public class UpdateCartValidator : Validator<UpdateCartRequest>
{
  public UpdateCartValidator()
  {
    RuleFor(c => c.Products).NotEmpty();
    RuleForEach(c => c.Products).SetValidator(new ProductValidator());
  }
}

public class CreateCartValidator : Validator<CreateNewCartRequest>
{
  public CreateCartValidator()
  {
    RuleFor(c => c.Products).NotEmpty();
    RuleForEach(c => c.Products).SetValidator(new ProductValidator());
  }
}

public class ProductValidator : Validator<Product>
{
  public ProductValidator()
  {
    RuleFor(x => x.Amount).GreaterThan(0);
  }
}
