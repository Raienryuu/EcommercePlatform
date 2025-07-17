using CartService.Models;
using CartService.Requests;
using FastEndpoints;
using FluentValidation;

namespace CartService.Validators;

public class UpdateCartValidator : Validator<UpdateCartRequest>
{
  public UpdateCartValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleForEach(x => x.Products).SetValidator(new ProductValidator());
  }
}

public class CreateCartValidator : Validator<CreateNewCartRequest>
{
  public CreateCartValidator()
  {
    _ = RuleFor(static c => c.Products).NotEmpty().WithMessage("Cart must contain at least one product");
    RuleForEach(static c => c.Products).SetValidator(new ProductValidator());
  }
}

public class ProductValidator : Validator<Product>
{
  public ProductValidator()
  {
    _ = RuleFor(static x => x.Id)
          .NotEmpty().WithMessage("Product ID is required")
          .Must(x => Guid.TryParse(x, out var r))
          .WithMessage("Product ID can only be Guid");

    _ = RuleFor(static x => x.Amount)
          .GreaterThan(0).WithMessage("Amount must be greater than 0")
          .LessThanOrEqualTo(100).WithMessage("Maximum quantity per product is 100");
  }
}
