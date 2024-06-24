using CartService.Requests;
using FastEndpoints;
using FluentValidation;

namespace CartService.Validators
{
  public class CartValidator : Validator<Cart>
  {
	public CartValidator()
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
}
