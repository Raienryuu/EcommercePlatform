using ImageService.Services;
using ImageService.Tests.Data;

namespace ImageService.Tests;

[ClassDataSource<MongoContainer>]
public class MongoProductImagesMetadataTests(MongoContainer mongoContainer)
{
  private MongoProductImagesMetadataService _mongoImageMetadataService = null!;

  [Before(Test)]
  public async Task SetUp()
  {
    _ = await mongoContainer.SeedMetadata();
    _mongoImageMetadataService = mongoContainer.GetImagesMetadataService();
  }

  [Test]
  [MethodDataSource(typeof(MetadataSamplesGenerator), nameof(MetadataSamplesGenerator.GetSeededMetadataIds))]
  public async Task GetProductImagesMetadataAsync_ExistingProductId_ProductMetadata(
       int productId)
  {
    var metadata = await _mongoImageMetadataService.GetProductImagesMetadataAsync(productId);

    _ = await Assert.That(metadata).IsNotNull();
    _ = await Assert.That(metadata.ProductId).IsEqualTo(productId);
  }
}
