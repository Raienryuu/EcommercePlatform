using FluentValidation.TestHelper;
using ProductService.Models;
using ProductService.Validation;
using Xunit;

namespace ProductService.Tests.Unit;

public class PaginationParamsValidatorTests
{
  private readonly PaginationParamsValidator _validator = new();

  [Theory]
  [InlineData(0)]
  [InlineData(201)]
  [InlineData(-5)]
  public void PaginationParamsValidator_PageSizeOutOfRange_ShouldHaveValidationError(int pageSize)
  {
    var model = new PaginationParams { PageSize = pageSize, PageNum = 1 };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.PageSize);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void PaginationParamsValidator_PageNumNotPositive_ShouldHaveValidationError(int pageNum)
  {
    var model = new PaginationParams { PageSize = 10, PageNum = pageNum };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.PageNum);
  }

  [Fact]
  public void PaginationParamsValidator_ValidParams_ShouldNotHaveAnyValidationErrors()
  {
    var model = new PaginationParams { PageSize = 10, PageNum = 1 };
    var result = _validator.TestValidate(model);
    result.ShouldNotHaveAnyValidationErrors();
  }
}