using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IImageService
{
  Task AddProductImageAsync(Guid productId, IFormFile file);
  Task<Image?> GetProductImageAsync(Guid productId, int imageNumber);
  /*public Task RemoveProductImage();*/
}
