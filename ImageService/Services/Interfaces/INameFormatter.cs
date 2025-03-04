
using ImageService.Models;

namespace ImageService.Services.Interfaces;
public interface INameFormatter
{
  string GetNameForProductImage(Guid productId, int imageNumber);
  int GetNumberOfNextImage(ProductImagesMetadata metadata);
  List<string> ResetImagesNumeration(List<string> storedImages);
}
