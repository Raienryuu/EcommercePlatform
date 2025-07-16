using Common;
using ImageService.Models;
using ImageService.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ImageService.Services;

public class MongoProductImagesMetadataService : IProductImagesMetadataService, IDisposable
{
  private readonly MongoClient _client;
  private readonly ConnectionOptions _options;
  private readonly IMongoCollection<ProductImagesMetadata> _metadataCollection;

  public MongoProductImagesMetadataService(IOptions<ConnectionOptions> options)
  {
    _options = options.Value;
    _client = new MongoClient(_options.ConnectionUri);
    _metadataCollection = _client
      .GetDatabase(_options.DatabaseName)
      .GetCollection<ProductImagesMetadata>("productImagesMetadata");
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

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }
}
