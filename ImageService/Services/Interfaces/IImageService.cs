using Common;
using ImageService.Models;

namespace ImageService.Services.Interfaces;

public interface IImageService
{
  Task<ServiceResult<int>> AddProductImageAsync(
    Guid productId,
    IFormFile file,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult<Image>> GetProductImageAsync(
    Guid productId,
    int imageNumber,
    int imageWidth,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> DeleteAllProductImages(Guid productId, CancellationToken cancellationToken = default);
  Task<ServiceResult<ProductImagesMetadata>> GetProductImagesMetadataAsync(
    Guid productId,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productMetadata);
  Task<ServiceResult> AddNewMetadataAsync(ProductImagesMetadata productMetadata);
  Task<ServiceResult> DeleteMetadataAsync(Guid productId, CancellationToken cancellationToken = default);
}
