using FluentValidation.TestHelper;
using IdentityService.Models;
using IdentityService.Models.Validators;
using Xunit;

namespace IdentityService.Tests.Unit;

public class UserAddressValidatorTests
{
  private readonly UserAddressValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_FullNameIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = value!,
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.FullName);
  }

  [Fact]
  public void UserAddressValidator_FullNameTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = new string('a', 201),
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.FullName);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("not-an-email")]
  public void UserAddressValidator_EmailIsInvalid_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = value!,
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Email);
  }

  [Fact]
  public void UserAddressValidator_EmailTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = new string('a', 201) + "@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Email);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_PhoneNumberIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = value!,
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
  }

  [Fact]
  public void UserAddressValidator_PhoneNumberTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = new string('1', 21),
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_AddressIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = value!,
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Address);
  }

  [Fact]
  public void UserAddressValidator_AddressTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = new string('a', 201),
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Address);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_CityIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = value!,
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.City);
  }

  [Fact]
  public void UserAddressValidator_CityTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = new string('a', 101),
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.City);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_ZIPCodeIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = value!,
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.ZIPCode);
  }

  [Fact]
  public void UserAddressValidator_ZIPCodeTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = new string('1', 21),
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.ZIPCode);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void UserAddressValidator_CountryIsEmpty_ShouldHaveValidationError(string? value)
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = value!
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Country);
  }

  [Fact]
  public void UserAddressValidator_CountryTooLong_ShouldHaveValidationError()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = new string('a', 101)
    };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.Country);
  }

  [Fact]
  public void UserAddressValidator_ValidModel_ShouldNotHaveAnyValidationErrors()
  {
    var model = new UserAddress
    {
      FullName = "John Doe",
      Email = "john.doe@mail.com",
      PhoneNumber = "+1234567890",
      Address = "123 Main St",
      City = "Sample City",
      ZIPCode = "12345",
      Country = "Sample Country"
    };
    var result = _validator.TestValidate(model);
    result.ShouldNotHaveAnyValidationErrors();
  }
}