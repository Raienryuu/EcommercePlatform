using FluentValidation;
using ProductService.Models;

namespace ProductService.Validation;

public class PaginationParamsValidator : AbstractValidator<PaginationParams>
{
  public PaginationParamsValidator()
  {
    RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    RuleFor(x => x.PageNum).GreaterThan(0);
  }
}
