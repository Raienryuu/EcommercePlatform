using ImageMagick;
using ImageService.Services.Interfaces;

namespace ImageService.Services;

public class ImageProcessor : IImageProcessor
{
  public byte[] ResizeToWidth(byte[] imageBytes, uint width)
  {
    var scaleSettings = new MagickGeometry() { Width = width };

    var image = new MagickImage(imageBytes);
    if (image.Width != width)
    {
      image.Scale(scaleSettings);
    }

    return image.ToByteArray();
  }
}
