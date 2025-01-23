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
    _metadataCollection = _client.GetDatabase(_options.DatabaseName)
        .GetCollection<ProductImagesMetadata>("productImagesMetadata");

  }

  public async Task<ProductImagesMetadata> GetProductImagesMetadataAsync(int productId)
  {
    var filter = Builders<ProductImagesMetadata>.Filter.Eq("ProductId", productId);
    return await _metadataCollection.Find(filter).FirstOrDefaultAsync() ?? new ProductImagesMetadata(productId, [], new NoMetadataAvailable());
  }
  public async Task AddNewMetadataAsync(ProductImagesMetadata productMetadata) => await _metadataCollection.InsertOneAsync(productMetadata);

  public async Task UpdateMetadataAsync(ProductImagesMetadata productMetadata)
  {
    var builder = Builders<ProductImagesMetadata>.Update;
    var updateDefinition = builder.Set(x => x.StoredImages, productMetadata.StoredImages);
    _ = await _metadataCollection.FindOneAndReplaceAsync(x => x.ProductId == productMetadata.ProductId, productMetadata);
  }

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }
}
