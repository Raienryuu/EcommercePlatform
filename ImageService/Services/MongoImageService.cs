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
  private readonly ILogger<MongoImageService> _logger;
  private readonly IMongoCollection<ProductImagesMetadata> _metadataCollection;

  public MongoImageService(
    IOptions<ConnectionOptions> connectionOptions,
    INameFormatter nameFormatter,
    ILogger<MongoImageService> logger
  )
  {
    _options = connectionOptions.Value;
    _client = new MongoClient(_options.ConnectionUri);
    _nameFormatter = nameFormatter;
    _logger = logger;
    _metadataCollection = _client
      .GetDatabase(_options.DatabaseName)
      .GetCollection<ProductImagesMetadata>("productImagesMetadata");
  }

  public async Task<ServiceResult<int>> AddProductImageAsync(
    Guid productId,
    IFormFile file,
    CancellationToken cancellationToken = default
  )
  {
    var productMetadataResult = await GetProductImagesMetadataAsync(productId, cancellationToken);

    if (productMetadataResult is { IsSuccess: false, ErrorMessage: not null })
    {
      return ServiceResults.Error<int>(productMetadataResult.ErrorMessage, productMetadataResult.StatusCode);
    }

    var collection = _client.GetDatabase(_options.DatabaseName).GetCollection<Image>("images");

    var imageBytes = new byte[file.Length];
    await file.OpenReadStream().ReadExactlyAsync(imageBytes, cancellationToken);

    var nextImageNumber = _nameFormatter.GetNumberOfNextImage(productMetadataResult.Value);

    var fileAsImage = new Image
    {
      ContentType = file.ContentType,
      Length = file.Length,
      Name = _nameFormatter.GetNameForProductImage(productId, nextImageNumber),
      Data = imageBytes,
    };

    productMetadataResult.Value.StoredImages.Add(fileAsImage.Name);

    var metadataUpdateTask = productMetadataResult.Value.MetadataState switch
    {
      NoMetadataAvailable => AddNewMetadataAsync(productMetadataResult.Value),
      MetadataAvailable => UpdateMetadataAsync(productMetadataResult.Value),
      _ => throw new InvalidOperationException("Invalid metadata state found."),
    };
    await metadataUpdateTask;
    await collection.InsertOneAsync(fileAsImage, cancellationToken: cancellationToken);

    return ServiceResults.Success(nextImageNumber, 200);
  }

  public async Task<ServiceResult<Image>> GetProductImageAsync(
    Guid productId,
    int imageNumber,
    int imageWidth,
    CancellationToken cancellationToken = default
  )
  {
    var collection = _client.GetDatabase(_options.DatabaseName).GetCollection<Image>("images");
    var fileName = _nameFormatter.GetNameForProductImage(productId, imageNumber);
    var filter = Builders<Image>.Filter.Eq("Name", fileName);
    var image = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return image is null ? ServiceResults.Error<Image>("Image not found", 404) : ServiceResults.Ok(image);
  }

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }

  public async Task<ServiceResult> DeleteAllProductImages(
    Guid productId,
    CancellationToken cancellationToken = default
  )
  {
    var imagesToDelete = await GetProductImagesMetadataAsync(productId, cancellationToken);
    if (imagesToDelete is { ErrorMessage: not null, IsFailure: true })
    {
      return ServiceResults.Error(imagesToDelete.ErrorMessage, imagesToDelete.StatusCode);
    }

    var collection = _client.GetDatabase(_options.DatabaseName).GetCollection<Image>("images");
    var filter = Builders<Image>.Filter.In("Name", imagesToDelete.Value.StoredImages);

    var deleteResult = await collection.DeleteManyAsync(filter, cancellationToken: cancellationToken);
    var deleteMetadataResult = await DeleteMetadataAsync(productId, cancellationToken);

    if (!deleteResult.IsAcknowledged || deleteMetadataResult.IsFailure)
    {
      return ServiceResults.Error("Couldn't delete images.", 500);
    }
    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult<ProductImagesMetadata>> GetProductImagesMetadataAsync(
    Guid productId,
    CancellationToken cancellationToken = default
  )
  {
    var filter = Builders<ProductImagesMetadata>.Filter.Eq("ProductId", productId);
    var metadata =
      await _metadataCollection.Find(filter).FirstOrDefaultAsync(cancellationToken)
      ?? new ProductImagesMetadata(productId, [], new NoMetadataAvailable());
    return ServiceResults.Success(metadata, 200);
  }

  public async Task<ServiceResult> AddNewMetadataAsync(ProductImagesMetadata productMetadata)
  {
    await _metadataCollection.InsertOneAsync(productMetadata);
    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productMetadata)
  {
    _ = await _metadataCollection.FindOneAndReplaceAsync(
      x => x.ProductId == productMetadata.ProductId,
      productMetadata
    );
    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult> DeleteMetadataAsync(
    Guid productId,
    CancellationToken cancellationToken = default
  )
  {
    var filter = Builders<ProductImagesMetadata>.Filter.Eq("ProductId", productId);
    var deleteResult = await _metadataCollection.DeleteOneAsync(filter, cancellationToken);
    if (deleteResult.IsAcknowledged)
    {
      return ServiceResults.Success(204);
    }
    return ServiceResults.Error("Coudln't delete product's metadata.", 500);
  }
}
