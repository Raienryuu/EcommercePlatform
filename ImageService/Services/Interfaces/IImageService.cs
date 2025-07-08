using Common;
using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IImageService
{
  Task<ServiceResult<int>> AddProductImageAsync(Guid productId, IFormFile file);
  Task<ServiceResult<Image>> GetProductImageAsync(Guid productId, int imageNumber);
  /*public Task RemoveProductImage();*/
}
