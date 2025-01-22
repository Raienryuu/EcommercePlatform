using ImageService.Models;
namespace ImageService.Tests.Data;

public class MetadataSamplesGenerator : DataSourceGeneratorAttribute<ProductImagesMetadata, int>
{
  public override IEnumerable<Func<(ProductImagesMetadata, int)>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
  {
    yield return static () => (new ProductImagesMetadata(5, ["p-5-0", "p-5-3"], new MetadataAvailable()), 1);
    yield return static () => (new ProductImagesMetadata(5, ["p-5-1", "p-5-2"], new MetadataAvailable()), 0);
    yield return static () => (new ProductImagesMetadata(5, [], new MetadataAvailable()), 0);
    yield return static () => (new ProductImagesMetadata(5, ["p-5-1"], new MetadataAvailable()), 0);
  }
}

