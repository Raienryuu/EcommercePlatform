using Common;
using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IProductImagesMetadataService
{
  Task<ServiceResult<ProductImagesMetadata>> GetProductImagesMetadataAsync(Guid productId, CancellationToken cancellationToken = default);
  Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productMetadata);
  Task<ServiceResult> AddNewMetadataAsync(ProductImagesMetadata productMetadata);
}

