using System.Diagnostics.CodeAnalysis;
using ImageService.Services;
using ImageService.Tests.Data;
using Microsoft.AspNetCore.Http;

namespace ImageService.Tests.Integration;

[ClassDataSource<MongoContainer>(Shared = SharedType.PerTestSession)]
[method: SetsRequiredMembers]
public class MongoImageServiceTests(MongoContainer mongoContainer)
{
  private MongoImageService _mongoImageService = null!;

  [Before(Test)]
  public void SetUp()
  {
    _mongoImageService = mongoContainer.GetImageService();
  }

  [Test]
  [SampleImagesGenerator]
  public async Task AddProductImageAsync_Image_FileSavedInDb(Guid productId, IFormFile file)
  {
    await _mongoImageService.AddProductImageAsync(productId, file);
    var image = await _mongoImageService.GetProductImageAsync(productId, 0, 0, default);

    _ = await Assert.That(image.Value).IsNotNull();
    _ = await Assert.That(image.Value.Length).IsEqualTo(file.Length);
  }

  [Test]
  [SampleImagesGenerator]
  public async Task GetProductImageAsync_ImageLocationDetails_FileFromDb(Guid productId, IFormFile file)
  {
    await _mongoImageService.AddProductImageAsync(productId, file);
    var image = await _mongoImageService.GetProductImageAsync(productId, 0, 0, default);

    _ = await Assert.That(image).IsNotNull();
    _ = await Assert.That(image.Value.Length).IsEqualTo(file.Length);
  }
}
