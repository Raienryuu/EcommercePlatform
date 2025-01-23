using System.Diagnostics.CodeAnalysis;
using ImageService.Services;
using ImageService.Tests.Data;
using Microsoft.AspNetCore.Http;

namespace ImageService.Tests;
[ClassDataSource<MongoContainer>]
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
  public async Task AddProductImageAsync_Image_FileSavedInDb(int productId, IFormFile file)
  {
    await _mongoImageService.AddProductImageAsync(productId, file);
    var image = await _mongoImageService.GetProductImageAsync(productId, 0);

    _ = await Assert.That(image).IsNotNull();
    _ = await Assert.That(image!.Length).IsEqualTo(file.Length);
  }
  [Test]
  [SampleImagesGenerator]
  public async Task GetProductImageAsync_ImageLocationDetails_FileFromDb(int productId, IFormFile file)
  {

    await _mongoImageService.AddProductImageAsync(productId, file);
    var image = await _mongoImageService.GetProductImageAsync(productId, 0);

    _ = await Assert.That(image).IsNotNull();
    _ = await Assert.That(image!.Length).IsEqualTo(file.Length);

  }
}
