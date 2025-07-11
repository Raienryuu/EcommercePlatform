using FluentValidation;

namespace IdentityService.Models.Validators;

public class UserAddressValidator : AbstractValidator<UserAddress>
{
  public UserAddressValidator()
  {
    RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
    RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
    RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    RuleFor(x => x.ZIPCode).NotEmpty().MaximumLength(20);
    RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
  }
}
