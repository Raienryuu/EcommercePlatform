using Common;
using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IImageService
{
  Task<ServiceResult<int>> AddProductImageAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default);
  Task<ServiceResult<Image>> GetProductImageAsync(Guid productId, int imageNumber, CancellationToken cancellationToken = default);
  /*public Task RemoveProductImage();*/
}
