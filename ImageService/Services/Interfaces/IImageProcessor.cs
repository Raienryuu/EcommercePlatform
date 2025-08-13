namespace ImageService.Services.Interfaces;

public interface IImageProcessor
{
  uint GetWidth(byte[] imageBytes);
  byte[] ResizeToWidth(byte[] imageBytes, uint width);
}
