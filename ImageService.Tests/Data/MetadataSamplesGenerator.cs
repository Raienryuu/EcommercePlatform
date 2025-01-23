using ImageService.Models;
namespace ImageService.Tests.Data;

public class MetadataSamplesGenerator : DataSourceGeneratorAttribute<ProductImagesMetadata, int>
{
  public override IEnumerable<Func<(ProductImagesMetadata, int)>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
  {
    yield return static () => (new ProductImagesMetadata(12, ["p-12-0", "p-12-3"], new MetadataAvailable()), 1);
    yield return static () => (new ProductImagesMetadata(5, ["p-5-1", "p-5-2"], new MetadataAvailable()), 0);
    yield return static () => (new ProductImagesMetadata(8, [], new MetadataAvailable()), 0);
    yield return static () => (new ProductImagesMetadata(30, ["p-30-1"], new MetadataAvailable()), 0);
  }

  public static IEnumerable<ProductImagesMetadata> GetSeedSamples()
  {
    yield return new ProductImagesMetadata(12, ["p-12-0", "p-12-3"], new MetadataAvailable());
    yield return new ProductImagesMetadata(5, ["p-5-1", "p-5-2"], new MetadataAvailable());
    yield return new ProductImagesMetadata(8, [], new MetadataAvailable());
    yield return new ProductImagesMetadata(30, ["p-30-1"], new MetadataAvailable());
  }

  public static IEnumerable<Func<int>> GetSeededMetadataIds()
  {
    yield return static () => 12;
    yield return static () => 5;
    yield return static () => 8;
    yield return static () => 30;
  }
}

