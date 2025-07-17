using FluentValidation.TestHelper;
using ProductService.Models;
using ProductService.Validation;

namespace ProductService.Tests.Unit;

public class ProductValidatorTests
{
  private readonly ProductValidator _validator;

  public ProductValidatorTests()
  {
    _validator = new ProductValidator();
  }

  [Fact]
  public void ProductValidator_CategoryIdIsZero_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 0,
      Name = "Test",
      Description = "Test",
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.CategoryId);
  }

  [Fact]
  public void ProductValidator_NameIsEmpty_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "",
      Description = "Test",
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Name);
  }

  [Fact]
  public void ProductValidator_NameTooLong_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = new string('A', 256),
      Description = "Test",
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Name);
  }

  [Fact]
  public void ProductValidator_DescriptionIsEmpty_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = "",
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Description);
  }

  [Fact]
  public void ProductValidator_DescriptionTooLong_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = new string('A', 1001),
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Description);
  }

  [Fact]
  public void ProductValidator_PriceIsZero_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = "Test",
      Price = 0,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Price);
  }

  [Fact]
  public void ProductValidator_PriceIsNegative_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = "Test",
      Price = -1,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Price);
  }

  [Fact]
  public void ProductValidator_QuantityIsNegative_ShouldHaveValidationError()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = "Test",
      Price = 10,
      Quantity = -1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldHaveValidationErrorFor(p => p.Quantity);
  }

  [Fact]
  public void ProductValidator_ValidProduct_ShouldNotHaveAnyValidationErrors()
  {
    // Arrange
    var product = new Product
    {
      CategoryId = 1,
      Name = "Test",
      Description = "Test",
      Price = 10,
      Quantity = 1,
    };

    // Act
    var result = _validator.TestValidate(product);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }
}
