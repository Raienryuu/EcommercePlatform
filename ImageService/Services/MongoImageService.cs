using System.Net;
using Common;
using ImageService.Models;
using ImageService.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ImageService.Services;

public class MongoImageService : IImageService, IDisposable
{
  private readonly MongoClient _client;
  private readonly ConnectionOptions _options;
  private readonly INameFormatter _nameFormatter;
  private readonly ILogger<MongoImageService> _logger;
  private IMongoCollection<ProductImagesMetadata> MetadataCollection =>
    _client.GetDatabase(_options.DatabaseName).GetCollection<ProductImagesMetadata>("productImagesMetadata");
  private readonly IImageProcessor _imageProcessor;

  public MongoImageService(
    IOptions<ConnectionOptions> connectionOptions,
    INameFormatter nameFormatter,
    IImageProcessor imageProcessor,
    ILogger<MongoImageService> logger
  )
  {
    _options = connectionOptions.Value;
    _client = new MongoClient(_options.ConnectionUri);
    _nameFormatter = nameFormatter;
    _logger = logger;
    _imageProcessor = imageProcessor;
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

    var imageWidth = _imageProcessor.GetWidth(imageBytes);

    var fileAsImage = new Image
    {
      ContentType = file.ContentType,
      Length = file.Length,
      Name = _nameFormatter.GetNameForProductImage(productId, nextImageNumber),
      Data = imageBytes,
      Width = imageWidth,
    };

    productMetadataResult.Value.StoredImages.Add(fileAsImage.Name);

    await UpdateMetadataAsync(productMetadataResult.Value);
    await collection.InsertOneAsync(fileAsImage, cancellationToken: cancellationToken);

    return ServiceResults.Success(nextImageNumber, 200);
  }

  public async Task<ServiceResult<Image>> GetProductImageAsync(
    Guid productId,
    int imageNumber,
    uint imageWidth,
    SizeResolveStrategy sizeStrategy,
    CancellationToken cancellationToken = default
  )
  {
    var fileName = _nameFormatter.GetNameForProductImage(productId, imageNumber);

    var collection = _client.GetDatabase(_options.DatabaseName).GetCollection<Image>("images");
    var image = sizeStrategy switch
    {
      SizeResolveStrategy.BestQuality => await GetBestQualityImage(collection, fileName, cancellationToken),
      SizeResolveStrategy.SmallestSize => await GetLowestSizeImage(collection, fileName, cancellationToken),
      SizeResolveStrategy.PreferQuality => await GetHigherQualityImage(
        collection,
        imageWidth,
        fileName,
        cancellationToken
      ),
      SizeResolveStrategy.PreferLowerSize => await GetLowerSizeImage(
        collection,
        imageWidth,
        fileName,
        cancellationToken
      ),
      _ => throw new NotImplementedException($"Size strategy not found, {sizeStrategy}"),
    };

    return image is null ? ServiceResults.Error<Image>("Image not found", 404) : ServiceResults.Ok(image);
  }

  private static Task<Image> GetBestQualityImage(
    IMongoCollection<Image> collection,
    string fileName,
    CancellationToken ct = default
  )
  {
    return collection
      .Aggregate()
      .Match(x => x.Name == fileName)
      .Sort(Builders<Image>.Sort.Descending("Width"))
      .FirstOrDefaultAsync(cancellationToken: ct);
  }

  private static Task<Image> GetLowestSizeImage(
    IMongoCollection<Image> collection,
    string fileName,
    CancellationToken ct = default
  )
  {
    return collection
      .Aggregate()
      .Match(x => x.Name == fileName)
      .Sort(Builders<Image>.Sort.Ascending("Width"))
      .FirstOrDefaultAsync(cancellationToken: ct);
  }

  private static async Task<Image> GetHigherQualityImage(
    IMongoCollection<Image> collection,
    uint width,
    string fileName,
    CancellationToken ct = default
  )
  {
    var pipeline = new[]
    {
      new BsonDocument("$match", new BsonDocument("Name", fileName)),
      new BsonDocument(
        "$addFields",
        new BsonDocument("diff", new BsonDocument("$subtract", new BsonArray { "$Width", width }))
      ),
      new BsonDocument("$match", new BsonDocument("diff", new BsonDocument("$lte", 0))),
      new BsonDocument("$sort", new BsonDocument("diff", 1)),
      new BsonDocument("$limit", 1),
      new BsonDocument(
        "$project",
        new BsonDocument
        {
          { "Name", 1 },
          { "Width", 1 },
          { "Data", 1 },
          { "Length", 1 },
          { "ContentType", 1 },
        }
      ),
    };
    var image =
      await collection
        .Aggregate<Image>(pipeline, cancellationToken: ct)
        .FirstOrDefaultAsync(cancellationToken: ct)
      ?? await GetLowerSizeImage(collection, width, fileName, ct);
    if (image is null)
    {
      return await GetBestQualityImage(collection, fileName, ct);
    }
    return image;
  }

  private static async Task<Image> GetLowerSizeImage(
    IMongoCollection<Image> collection,
    uint width,
    string fileName,
    CancellationToken ct = default
  )
  {
    var pipeline = new[] // TODO don't use this, make use of metadata instead as it is being pulled in anyway
    {
      new BsonDocument("$match", new BsonDocument("Name", fileName)),
      new BsonDocument(
        "$addFields",
        new BsonDocument("diff", new BsonDocument("$subtract", new BsonArray { "$Width", width }))
      ),
      new BsonDocument("$match", new BsonDocument("diff", new BsonDocument("$gte", 0))),
      new BsonDocument("$sort", new BsonDocument("diff", 1)),
      new BsonDocument("$limit", 1),
      new BsonDocument(
        "$project",
        new BsonDocument
        {
          { "Name", 1 },
          { "Width", 1 },
          { "Data", 1 },
          { "Length", 1 },
          { "ContentType", 1 },
        }
      ),
    };
    var image =
      await collection
        .Aggregate<Image>(pipeline, cancellationToken: ct)
        .FirstOrDefaultAsync(cancellationToken: ct)
      ?? await GetLowerSizeImage(collection, width, fileName, ct);
    if (image is null)
    {
      return await GetBestQualityImage(collection, fileName, ct);
    }
    return image;
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
      await MetadataCollection.Find(filter).FirstOrDefaultAsync(cancellationToken)
      ?? new ProductImagesMetadata(productId, [], new NoMetadataAvailable());
    return ServiceResults.Success(metadata, 200);
  }

  public async Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productImagesMetadata)
  {
    var metadataUpdateTask = productImagesMetadata.MetadataState switch
    {
      NoMetadataAvailable => MetadataCollection.InsertOneAsync(productImagesMetadata),
      MetadataAvailable => MetadataCollection.FindOneAndReplaceAsync(
        x => x.ProductId == productImagesMetadata.ProductId,
        productImagesMetadata
      ),
      _ => throw new InvalidOperationException("Invalid metadata state found."),
    };

    await metadataUpdateTask;

    return ServiceResults.Success(200);
  }

  public async Task<ServiceResult> DeleteMetadataAsync(
    Guid productId,
    CancellationToken cancellationToken = default
  )
  {
    var filter = Builders<ProductImagesMetadata>.Filter.Eq("ProductId", productId);
    var deleteResult = await MetadataCollection.DeleteOneAsync(filter, cancellationToken);
    if (deleteResult.IsAcknowledged)
    {
      return ServiceResults.Success(204);
    }
    return ServiceResults.Error("Coudln't delete product's metadata.", 500);
  }

  public async Task<ServiceResult<List<string>>> AddScaledProductImageAsync(
    Guid productId,
    IFormFile file,
    CancellationToken cancellationToken = default,
    params uint[] dimensions
  )
  {
    var productImagesMetadata = await GetProductImagesMetadataAsync(productId, cancellationToken);

    if (productImagesMetadata is { IsSuccess: false, ErrorMessage: not null })
    {
      return productImagesMetadata.RemapError<ProductImagesMetadata, List<string>>();
    }

    var collection = _client.GetDatabase(_options.DatabaseName).GetCollection<Image>("images");

    var imageBytes = new byte[file.Length]; // might be critical to do it inside loop if it's being mutated
    await file.OpenReadStream().ReadExactlyAsync(imageBytes, cancellationToken);

    var nextImageNumber = _nameFormatter.GetNumberOfNextImage(productImagesMetadata.Value);
    var processedDimensions = 0;

    try
    {
      foreach (var dimension in dimensions)
      {
        var imageName = _nameFormatter.GetNameForProductImage(productId, nextImageNumber);

        var scaledImage = _imageProcessor.ResizeToWidth(imageBytes, dimension);
        var fileAsImage = new Image
        {
          ContentType = file.ContentType,
          Length = file.Length,
          Name = imageName,
          Data = scaledImage,
          Width = dimension,
        };
        await collection.InsertOneAsync(fileAsImage, cancellationToken: cancellationToken);
        productImagesMetadata.Value.StoredImages.Add(fileAsImage.Name);
        processedDimensions++;
        nextImageNumber++;
      }
      await UpdateMetadataAsync(productImagesMetadata.Value);
    }
    catch (MongoException e)
    {
      _logger.LogCritical(
        "Unable to store scaled image information, images already processed {processed} out of {numberOfDimensions}, exception: {ex}",
        processedDimensions,
        dimensions.Length,
        e
      );
      return ServiceResults.Error<List<string>>(
        "Unable to store scaled image information. Try again later.",
        (int)HttpStatusCode.ServiceUnavailable
      );
    }

    return ServiceResults.Success(productImagesMetadata.Value.StoredImages, (int)HttpStatusCode.Created);
  }
}
