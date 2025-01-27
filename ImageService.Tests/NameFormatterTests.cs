using ImageService.Services;
using ImageService.Models;
using ImageService.Tests.Data;

namespace ImageService.Tests;

internal class NameFormatterTests
{

  private readonly NameFormatter _formatter = new();

  [Test]
  [Arguments(2, 5, "p-2-5")]
  [Arguments(122, 50, "p-122-50")]
  public async Task GetNameForProductImage_ProductIdAndImageNumber_CorretclyFormattedString(int productId, int imageNumber, string expected)
  {
    var result = _formatter.GetNameForProductImage(productId, imageNumber);

    _ = await Assert.That(result).IsEqualTo(expected);
  }

  [Test]
  [MetadataSamplesGenerator]
  public async Task GetNumberOfNextImage_ValidProductMetadata_CalculatedNumberForNextImage(ProductImagesMetadata metadata, int expected)
  {
    var result = _formatter.GetNumberOfNextImage(metadata);

    _ = await Assert.That(result).IsEqualTo(expected);
  }


}
