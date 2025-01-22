using ImageService.Services;
using ImageService.Tests.Data;
using Microsoft.AspNetCore.Http;

namespace ImageService.Tests;

public class MongoImageServiceTests
{

  private readonly MongoImageService _mongoService;

  public MongoImageServiceTests()
  {
    var options = new ConnectionOptions
    {
      ConnectionUri = "get from testcontainers",
      DatabaseName = "mongoImageService-" + Guid.NewGuid(),
    };

    _mongoService = new(options, new NameFormatter(), new MongoProductImagesMetadataService(options));
  }

  [Test]
  [SampleImagesGenerator]
  public async Task AddProductImageAsync_(int productId, IFormFile file)
  {
    _mongoService.AddProductImageAsync(productId, file);
  }


  /*GetProductImageAsync*/
}
