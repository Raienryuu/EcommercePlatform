using Common;
using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IProductImagesMetadataService
{
  Task<ServiceResult<ProductImagesMetadata>> GetProductImagesMetadataAsync(Guid productId);
  Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productMetadata);
  Task<ServiceResult> AddNewMetadataAsync(ProductImagesMetadata productMetadata);
}

