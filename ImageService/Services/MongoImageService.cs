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

  private const string DB_NAME = "ecommerce";

  public MongoImageService(IOptions<ConnectionOptions> connectionOptions, INameFormatter nameFormatter, IProductImagesMetadataService metadataService)
  {
    _options = connectionOptions.Value;
    _client = new MongoClient(_options.ConnectionUri);
    _nameFormatter = nameFormatter;
    _metadataService = metadataService;
  }

  // try mitigate copying data to byte[] ??
  public async Task AddProductImageAsync(Guid productId, IFormFile file)
  {
    var productMetadata = await _metadataService.GetProductImagesMetadataAsync(productId);

    var collection = _client.GetDatabase(DB_NAME)
        .GetCollection<Image>("images");

    var imageBytes = new byte[file.Length];
    await file.OpenReadStream().ReadExactlyAsync(imageBytes);

    var nextImageNumber = _nameFormatter.GetNumberOfNextImage(productMetadata);

    var fileAsImage = new Image
    {
      ContentType = file.ContentType,
      Length = file.Length,
      Name = _nameFormatter.GetNameForProductImage(productId, nextImageNumber),
      Data = imageBytes,
    };

    productMetadata.StoredImages.Add(fileAsImage.Name);
    productMetadata.MetadataState.Apply(_metadataService, productMetadata);
    await collection.InsertOneAsync(fileAsImage);
  }

  public async Task<Image?> GetProductImageAsync(Guid productId, int imageNumber)
  {
    var collection = _client.GetDatabase(DB_NAME)
        .GetCollection<Image>("images");
    var fileName = _nameFormatter.GetNameForProductImage(productId, imageNumber);
    var filter = Builders<Image>.Filter.Eq("Name", fileName);
    return await collection.Find(filter).FirstOrDefaultAsync();
  }

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }
}
