
using ImageService.Models;
using ImageService.Services.Interfaces;

namespace ImageService.Services;
public class NameFormatter : INameFormatter
{
    // Gets name in format: p-@productId-@imageNumber
    public string GetNameForProductImage(int productId, int imageNumber)
    {
        const string Prefix = "p-";
        string Suffix = '-' + imageNumber.ToString();
        return Prefix + productId.ToString() + Suffix;
    }

    public int GetNumberOfNextImage(ProductImagesMetadata metadata)
    {
        if (metadata.StoredImages.Count == 0) return 0;

        List<int> imagesNumbers = [];

        metadata.StoredImages.Sort(FormattedComparer);

        int proposedNumber = 0;
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
        List<string> fixedImagesNumeration = [];
        int currentNumber = 0;
        foreach (var name in storedImages)
        {
            var imageNumber = GetImageNumber(name);
            if (imageNumber != currentNumber)
            {

            }

        }
        throw new NotImplementedException();
    }
    private static int GetImageNumber(string formattedProductName)
    {
        return Int32.Parse(formattedProductName.Split('-')[2]);
    }

    private static int FormattedComparer(string x, string y)
    {
        var result = GetImageNumber(x) - GetImageNumber(y);
        if (result < 0)
            return -1;
        if (result == 0)
            return 0;
        return 1;

    }

}
