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
  Task<ServiceResult<List<string>>> AddScaledProductImageAsync(
    Guid productId,
    IFormFile file,
    CancellationToken cancellationToken = default,
    params int[] dimensions
  );
  Task<ServiceResult<Image>> GetProductImageAsync(
    Guid productId,
    int imageNumber,
    int imageWidth,
    SizeResolveStrategy sizeStrategy,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> DeleteAllProductImages(Guid productId, CancellationToken cancellationToken = default);
  Task<ServiceResult<ProductImagesMetadata>> GetProductImagesMetadataAsync(
    Guid productId,
    CancellationToken cancellationToken = default
  );
  Task<ServiceResult> UpdateMetadataAsync(ProductImagesMetadata productMetadata);
  Task<ServiceResult> DeleteMetadataAsync(Guid productId, CancellationToken cancellationToken = default);
}
