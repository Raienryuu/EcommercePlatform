using ImageService.Models;
using ImageService.Services;
using ImageService.Tests.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using TUnit.Core.Interfaces;

namespace ImageService.Tests;

public class MongoContainer : IAsyncInitializer, IAsyncDisposable
{
  private IOptions<ConnectionOptions> _options = null!;
  private const string META_COLLECTION_NAME = "productImagesMetadata";

  public static string GetMetaCollectionName() => META_COLLECTION_NAME;

  private readonly MongoDbContainer _dbContainer = new MongoDbBuilder()
    .WithImage("mongo:7.0.9")
    .WithPortBinding(27017, true)
    .Build();

  public IOptions<ConnectionOptions> GetConnectionOptions()
  {
    if (_options is not null)
    {
      return _options;
    }
    var options = new ConnectionOptions()
    {
      ConnectionUri = _dbContainer.GetConnectionString(),
      DatabaseName = "mongoImageService-" + Guid.NewGuid(),
    };
    _options = Options.Create(options);
    return _options;
  }

  public string GetConnectionString() => _dbContainer.GetConnectionString();

  public MongoImageService GetImageService() =>
    new(
      GetConnectionOptions(),
      new NameFormatter(),
      new MongoProductImagesMetadataService(GetConnectionOptions())
    );

  public MongoProductImagesMetadataService GetImagesMetadataService() => new(GetConnectionOptions());

  public async ValueTask DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();
  }
}

public static class MongoContainerExtensions
{
  public static async Task<MongoContainer> SeedMetadata(this MongoContainer container)
  {
    var client = new MongoClient(container.GetConnectionOptions().Value.ConnectionUri);
    var collectionName = MongoContainer.GetMetaCollectionName();
    var dbName = container.GetConnectionOptions().Value.DatabaseName;

    await client
      .GetDatabase(dbName)
      .GetCollection<ProductImagesMetadata>(collectionName)
      .InsertManyAsync(MetadataSamplesGenerator.GetSeedSamples());
    return container;
  }
}
