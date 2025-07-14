using ImageService.Models;
using ImageService.Services;
using ImageService.Tests.Data;

namespace ImageService.Tests;

internal class NameFormatterTests
{
  private readonly NameFormatter _formatter = new();

  [Test]
  [Arguments("2d67ddad-5e05-4e22-99e5-78d175485ceb", 5, "p-2d67ddad-5e05-4e22-99e5-78d175485ceb-5")]
  [Arguments("a13957c1-22de-4e44-84fe-8f84782cc845", 50, "p-a13957c1-22de-4e44-84fe-8f84782cc845-50")]
  public async Task GetNameForProductImage_ProductIdAndImageNumber_CorrectlyFormattedString(
    string productId,
    int imageNumber,
    string expected
  )
  {
    var guid = Guid.Parse(productId);
    var result = _formatter.GetNameForProductImage(guid, imageNumber);

    _ = await Assert.That(result).IsEqualTo(expected);
  }

  [Test]
  [MetadataSamplesGenerator]
  public async Task GetNumberOfNextImage_ValidProductMetadata_CalculatedNumberForNextImage(
    ProductImagesMetadata metadata,
    int expected
  )
  {
    var result = _formatter.GetNumberOfNextImage(metadata);

    _ = await Assert.That(result).IsEqualTo(expected);
  }
}
