using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IProductImagesMetadataService
{
    public Task<ProductImagesMetadata> GetProductImagesMetadataAsync(int productId);
    public Task UpdateMetadataAsync(ProductImagesMetadata productMetadata);
    public Task AddNewMetadataAsync(ProductImagesMetadata productMetadata);
}

