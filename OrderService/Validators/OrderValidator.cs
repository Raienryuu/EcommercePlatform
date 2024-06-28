using FluentValidation;
using OrderService.Models;

namespace OrderService.Validators
{
  public class OrderValidator : AbstractValidator<Order>
  {
	public OrderValidator()
	{
	  RuleFor(x => x.Products).NotEmpty();
	  RuleForEach(x => x.Products).SetValidator(new OrderProductValidator());
	}
  }

  public class OrderProductValidator : AbstractValidator<OrderProduct>
  {
	public OrderProductValidator()
	{
	  RuleFor(x => x.Quantity).GreaterThan(0);
	}
  }
}


