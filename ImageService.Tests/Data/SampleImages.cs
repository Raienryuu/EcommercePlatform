using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace ImageService.Tests.Data;

public class SampleImagesGenerator : DataSourceGeneratorAttribute<Guid, IFormFile>
{
  private readonly Guid _productId = Guid.NewGuid();

  public override IEnumerable<Func<(Guid, IFormFile)>> GenerateDataSources(
    DataGeneratorMetadata dataGeneratorMetadata
  )
  {
    var file = File.OpenRead("../../../../UserClient/src/assets/logo_DHL.jpg"); // MIGHT DISAPPEAR SOMEDAY
    var formFile = new FormFile(file, 0, file.Length, "file.jpg", "file")
    {
      Headers = new HeaderDictionary(),
      ContentType = MediaTypeNames.Image.Jpeg,
    };
    yield return () => (_productId, formFile);
  }
}
