using FluentValidation.TestHelper;
using IdentityService.Models;
using IdentityService.Models.Validators;
using Xunit;

namespace IdentityService.Tests.Unit;

public class NewUserValidatorTests
{
  private readonly NewUserValidator _validator = new();

  [Fact]
  public void NewUserValidator_UserNameIsEmpty_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = "", Password = "validPassword", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.UserName);
  }

  [Fact]
  public void NewUserValidator_UserNameTooShort_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = "ab", Password = "validPassword", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.UserName);
  }

  [Fact]
  public void NewUserValidator_UserNameTooLong_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = new string('a', 51), Password = "validPassword", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.UserName);
  }

  [Fact]
  public void NewUserValidator_PasswordIsEmpty_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = "validUser", Password = "", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Fact]
  public void NewUserValidator_PasswordTooShort_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = "validUser", Password = "12345", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Fact]
  public void NewUserValidator_PasswordTooLong_ShouldHaveValidationError()
  {
    var model = new NewUser { UserName = "validUser", Password = new string('a', 101), Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Fact]
  public void NewUserValidator_ValidModel_ShouldNotHaveAnyValidationErrors()
  {
    var model = new NewUser { UserName = "validUser", Password = "validPassword", Address = ValidAddress() };
    var result = _validator.TestValidate(model);
    result.ShouldNotHaveAnyValidationErrors();
  }

  private static UserAddress ValidAddress() => new UserAddress
  {
    FullName = "John Doe",
    Email = "john.doe@mail.com",
    PhoneNumber = "+1234567890",
    Address = "123 Main St",
    City = "Sample City",
    ZIPCode = "12345",
    Country = "Sample Country"
  };
}