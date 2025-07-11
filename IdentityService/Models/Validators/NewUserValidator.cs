using FluentValidation;

namespace IdentityService.Models.Validators;

public class NewUserValidator : AbstractValidator<NewUser>
{
  public NewUserValidator()
  {
    RuleFor(user => user.UserName).NotEmpty().MinimumLength(3).MaximumLength(50);

    RuleFor(user => user.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
  }
}
