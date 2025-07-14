using ImageService.Models;

namespace ImageService.Tests.Data;

public class MetadataSamplesGenerator : DataSourceGeneratorAttribute<ProductImagesMetadata, int>
{
  public override IEnumerable<Func<(ProductImagesMetadata, int)>> GenerateDataSources(
    DataGeneratorMetadata dataGeneratorMetadata
  )
  {
    yield return static () =>
      (
        new ProductImagesMetadata(
          Guid.Parse("d7188d1d-4c98-497e-b8bb-5a1642d087d8"),
          ["p-d7188d1d-4c98-497e-b8bb-5a1642d087d8-0", "p-d7188d1d-4c98-497e-b8bb-5a1642d087d8-3"],
          new MetadataAvailable()
        ),
        1
      );
    yield return static () =>
      (
        new ProductImagesMetadata(
          Guid.Parse("2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d"),
          ["p-2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d-1", "p-2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d-2"],
          new MetadataAvailable()
        ),
        0
      );
    yield return static () =>
      (
        new ProductImagesMetadata(
          Guid.Parse("5c61ab74-a51a-4d59-aef5-46ba528186ea"),
          [],
          new MetadataAvailable()
        ),
        0
      );
    yield return static () =>
      (
        new ProductImagesMetadata(
          Guid.Parse("5ad5cc94-8235-4480-82bd-5400981ab874"),
          ["p-5ad5cc94-8235-4480-82bd-5400981ab874-1"],
          new MetadataAvailable()
        ),
        0
      );
  }

  public static IEnumerable<ProductImagesMetadata> GetSeedSamples()
  {
    yield return new ProductImagesMetadata(
      Guid.Parse("d7188d1d-4c98-497e-b8bb-5a1642d087d8"),
      ["p-d7188d1d-4c98-497e-b8bb-5a1642d087d8-0", "p-d7188d1d-4c98-497e-b8bb-5a1642d087d8-3"],
      new MetadataAvailable()
    );
    yield return new ProductImagesMetadata(
      Guid.Parse("2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d"),
      ["p-2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d-1", "p-2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d-2"],
      new MetadataAvailable()
    );
    yield return new ProductImagesMetadata(
      Guid.Parse("5c61ab74-a51a-4d59-aef5-46ba528186ea"),
      [],
      new MetadataAvailable()
    );
    yield return new ProductImagesMetadata(
      Guid.Parse("5ad5cc94-8235-4480-82bd-5400981ab874"),
      ["p-5ad5cc94-8235-4480-82bd-5400981ab874-1"],
      new MetadataAvailable()
    );
  }

  public static IEnumerable<Func<Guid>> GetSeededMetadataIds()
  {
    yield return static () => Guid.Parse("d7188d1d-4c98-497e-b8bb-5a1642d087d8");
    yield return static () => Guid.Parse("2b422f5f-5ba4-4e8b-ad0b-3bd90787cc3d");
    yield return static () => Guid.Parse("5c61ab74-a51a-4d59-aef5-46ba528186ea");
    yield return static () => Guid.Parse("5ad5cc94-8235-4480-82bd-5400981ab874");
  }
}
