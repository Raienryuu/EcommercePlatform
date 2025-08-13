using ImageMagick;
using ImageService.Services.Interfaces;

namespace ImageService.Services;

public class ImageProcessor : IImageProcessor
{
  public uint GetWidth(byte[] imageBytes)
  {
    var image = new MagickImage(imageBytes);
    return image.Width;
  }

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
