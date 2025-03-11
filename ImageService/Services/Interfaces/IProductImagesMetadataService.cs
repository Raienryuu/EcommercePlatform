using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IProductImagesMetadataService
{
  Task<ProductImagesMetadata> GetProductImagesMetadataAsync(Guid productId);
  Task UpdateMetadataAsync(ProductImagesMetadata productMetadata);
  Task AddNewMetadataAsync(ProductImagesMetadata productMetadata);
}

