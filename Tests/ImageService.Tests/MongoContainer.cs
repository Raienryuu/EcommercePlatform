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
  private const string META_COLLECTION_NAME = "productImagesMetadata";
  private readonly string _databaseName = "mongoImageService-" + Guid.NewGuid();

  public static string GetMetaCollectionName() => META_COLLECTION_NAME;

  private static readonly MongoDbContainer s_dbContainer = new MongoDbBuilder()
    .WithImage("mongo:7.0.9")
    .WithPortBinding(27017, true)
    .Build();

  public IOptions<ConnectionOptions> GetConnectionOptions()
  {
    var options = new ConnectionOptions()
    {
      ConnectionUri = s_dbContainer.GetConnectionString(),
      DatabaseName = _databaseName,
    };
    return Options.Create(options);
  }

  public static string GetConnectionString() => s_dbContainer.GetConnectionString();

  public MongoImageService GetImageService() =>
    new(
      GetConnectionOptions(),
      new NameFormatter(),
      new MongoProductImagesMetadataService(GetConnectionOptions())
    );

  public MongoProductImagesMetadataService GetImagesMetadataService() => new(GetConnectionOptions());

  public async ValueTask DisposeAsync()
  {
    await s_dbContainer.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  public async Task InitializeAsync()
  {
    await s_dbContainer.StartAsync();
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
