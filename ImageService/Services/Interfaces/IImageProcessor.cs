namespace ImageService.Services.Interfaces;

public interface IImageProcessor
{
  public byte[] ResizeToWidth(byte[] imageBytes, uint width);
}
