// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly
using ImageService.Models;
using MongoDB.Bson.Serialization;

[assembly: Retry(3)]

namespace ImageService.Tests;

public class GlobalHooks
{
  [Before(TestSession, nameof(MongoProductImagesMetadataTests))]
  public static void SetUp()
  {
    _ = BsonClassMap.RegisterClassMap<ProductImagesMetadata>(static map =>
    {
      _ = map.MapCreator(static p => new ProductImagesMetadata(
        p.ProductId,
        p.StoredImages,
        new MetadataAvailable()
      ));
      map.AutoMap();
      map.UnmapProperty(static p => p.MetadataState);
    });
  }

  [After(TestSession)]
  public static void CleanUp() { }
}
