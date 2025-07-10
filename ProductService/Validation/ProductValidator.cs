using FluentValidation;
using ProductService.Models;

namespace ProductService.Validation;

public class ProductValidator : AbstractValidator<Product>
{
  public ProductValidator()
  {
    RuleFor(p => p.CategoryId).NotEmpty().WithMessage("Category ID is required."); // Make sure CategoryId is not zero
    RuleFor(p => p.Name)
      .NotEmpty()
      .MaximumLength(255)
      .WithMessage("Name must not be empty or exceed 255 characters.");
    RuleFor(p => p.Description)
      .NotEmpty()
      .MaximumLength(1000)
      .WithMessage("Description must not be empty or exceed 1000 characters.");

    // Using Must() to apply the same validation logic as the Data Annotations.
    RuleFor(p => p.Price)
      .Must(BeAValidPrice)
      .WithMessage("Price must be a positive value greater than 0.01.");
    RuleFor(p => p.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be a non-negative value."); //Fluent validator already check for non negative quantity
  }

  private bool BeAValidPrice(decimal price)
  {
    return price >= 0.01m;
  }
}
