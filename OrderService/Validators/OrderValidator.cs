using FluentValidation;
using OrderService.Endpoints.Requests;
using OrderService.Models;

namespace OrderService.Validators
{
  public class OrderValidator : AbstractValidator<CreateOrderRequest>
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
