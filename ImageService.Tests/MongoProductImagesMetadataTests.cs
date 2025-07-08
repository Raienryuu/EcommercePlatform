using ImageService.Models;
using ImageService.Services;
using ImageService.Tests.Data;

namespace ImageService.Tests;

[ClassDataSource<MongoContainer>(Shared = SharedType.PerTestSession)]
public class MongoProductImagesMetadataTests(MongoContainer mongoContainer)
{
  private MongoProductImagesMetadataService _mongoImageMetadataService = null!;

  [Before(Test)]
  public async Task SetUp()
  {
    _mongoImageMetadataService = mongoContainer.GetImagesMetadataService();
    _ = await mongoContainer.SeedMetadata();
  }

  [Test]
  [MethodDataSource(typeof(MetadataSamplesGenerator), nameof(MetadataSamplesGenerator.GetSeededMetadataIds))]
  public async Task GetProductImagesMetadataAsync_ExistingProductId_ProductMetadata(Guid productId)
  {
    var metadata = await _mongoImageMetadataService.GetProductImagesMetadataAsync(productId);

    _ = await Assert.That(metadata).IsNotNull();
    _ = await Assert.That(metadata.Value.ProductId).IsEqualTo(productId);
    _ = await Assert.That(metadata.Value.MetadataState).IsTypeOf<MetadataAvailable>();
  }
}
