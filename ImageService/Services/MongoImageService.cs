using Common;
using ImageService.Models;
using ImageService.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ImageService.Services;

public class MongoImageService : IImageService, IDisposable
{
  private readonly MongoClient _client;
  private readonly ConnectionOptions _options;
  private readonly INameFormatter _nameFormatter;
  private readonly IProductImagesMetadataService _metadataService;


  public MongoImageService(IOptions<ConnectionOptions> connectionOptions, INameFormatter nameFormatter, IProductImagesMetadataService metadataService)
  {
    _options = connectionOptions.Value;
    _client = new MongoClient(_options.ConnectionUri);
    _nameFormatter = nameFormatter;
    _metadataService = metadataService;
  }


  public async Task<ServiceResult<int>> AddProductImageAsync(Guid productId, IFormFile file)
  {
    var productMetadataResult = await _metadataService.GetProductImagesMetadataAsync(productId);
    if (productMetadataResult is { IsSuccess: false, ErrorMessage: not null })
    {
      return ServiceResults.Error<int>(productMetadataResult.ErrorMessage,
        productMetadataResult.StatusCode);
    }

    var collection = _client.GetDatabase(_options.DatabaseName)
        .GetCollection<Image>("images");

    var imageBytes = new byte[file.Length];
    await file.OpenReadStream().ReadExactlyAsync(imageBytes);

    var nextImageNumber = _nameFormatter.GetNumberOfNextImage(productMetadataResult.Value);

    var fileAsImage = new Image
    {
      ContentType = file.ContentType,
      Length = file.Length,
      Name = _nameFormatter.GetNameForProductImage(productId, nextImageNumber),
      Data = imageBytes,
    };

    productMetadataResult.Value.StoredImages.Add(fileAsImage.Name);
    productMetadataResult.Value.MetadataState.Apply(_metadataService, productMetadataResult.Value);
    await collection.InsertOneAsync(fileAsImage);
    return ServiceResults.Success(nextImageNumber, 200);
  }

  public async Task<ServiceResult<Image>> GetProductImageAsync(Guid productId, int imageNumber)
  {
    var collection = _client.GetDatabase(_options.DatabaseName)
        .GetCollection<Image>("images");
    var fileName = _nameFormatter.GetNameForProductImage(productId, imageNumber);
    var filter = Builders<Image>.Filter.Eq("Name", fileName);
    var image = await collection.Find(filter).FirstOrDefaultAsync();
    return image is null ?  ServiceResults.Error<Image>("Image not found", 404) :
      ServiceResults.Ok(image);
  }

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }
}
