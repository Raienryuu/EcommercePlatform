using System.Globalization;
using ImageService.Models;
using ImageService.Services.Interfaces;

namespace ImageService.Services;
public class NameFormatter : INameFormatter
{
  // Gets name in format: p-@productId-@imageNumber
  public string GetNameForProductImage(Guid productId, int imageNumber)
  {
    const string PREFIX = "p-";
    var suffix = '-' + imageNumber.ToString(CultureInfo.InvariantCulture);
    return PREFIX + productId.ToString() + suffix;
  }

  public int GetNumberOfNextImage(ProductImagesMetadata metadata)
  {
    if (metadata.StoredImages.Count == 0)
    {
      return 0;
    }

    List<int> imagesNumbers = [];

    metadata.StoredImages.Sort(FormattedComparer);

    var proposedNumber = 0;
    foreach (var image in metadata.StoredImages)
    {
      var number = GetImageNumber(image);
      if (proposedNumber != number)
      {
        break;
      }
      else
      {
        proposedNumber++;
      }
    }

    return proposedNumber;
  }

  public List<string> ResetImagesNumeration(List<string> storedImages)
  {
    throw new NotImplementedException();
    var currentNumber = 0;
    foreach (var name in storedImages)
    {
      var imageNumber = GetImageNumber(name);
      if (imageNumber != currentNumber)
      {

      }
    }
  }
  private static int GetImageNumber(string formattedProductName) => int.Parse(formattedProductName.Split('-')[^1], CultureInfo.InvariantCulture);

  private static int FormattedComparer(string x, string y)
  {
    var result = GetImageNumber(x) - GetImageNumber(y);

    return result switch
    {
      < 0 => -1,
      0 => 0,
      _ => result
    };
  }
}

