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
	}
  }
}
