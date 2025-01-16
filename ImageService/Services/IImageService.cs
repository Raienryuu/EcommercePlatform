namespace ImageService.Services;

public interface IImageService
{
    public Task GetProductImagesMetadata();
    public Task AddProductImage(int productId, IFormFile file);
    public Task<IFormFile> GetProductImage(int productId, int imageNumber);
    /*public Task RemoveProductImage();*/
}
