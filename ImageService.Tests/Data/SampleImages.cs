using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace ImageService.Tests.Data;

public class SampleImagesGenerator : DataSourceGeneratorAttribute<int, IFormFile>
{
  private const int PRODUCT_ID = 15;
  public override IEnumerable<Func<(int, IFormFile)>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
  {

    var file = File.OpenRead("../../../../UserClient/src/assets/logo_DHL.jpg"); // MIGHT DISAPPEAR SOMEDAY
    var formFile = new FormFile(file, 0, file.Length, "file.jpg", "file")
    {
      Headers = new HeaderDictionary(),
      ContentType = MediaTypeNames.Image.Jpeg,
    };
    yield return () => (PRODUCT_ID, formFile);
  }
}
