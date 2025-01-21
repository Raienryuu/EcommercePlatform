using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IImageService
{
    public Task AddProductImageAsync(int productId, IFormFile file);
    public Task<Image?> GetProductImageAsync(int productId, int imageNumber);
    /*public Task RemoveProductImage();*/
}
