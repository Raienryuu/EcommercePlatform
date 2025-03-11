using System.Diagnostics.CodeAnalysis;
using ImageService.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ImageService.Models;

[method: SetsRequiredMembers]
public class ProductImagesMetadata(Guid productId, List<string> storedImages, IMetadataState state)
{
  public ObjectId Id { get; set; }

  [BsonGuidRepresentation(GuidRepresentation.Standard)]
  public required Guid ProductId { get; init; } = productId;
  public List<string> StoredImages { get; set; } = storedImages;
  public required IMetadataState MetadataState { get; set; } = state;
}

public interface IMetadataState
{
  void Apply(IProductImagesMetadataService metadataService, ProductImagesMetadata p);
}

public class NoMetadataAvailable : IMetadataState
{
  public void Apply(IProductImagesMetadataService metadataService, ProductImagesMetadata p) =>
    metadataService.AddNewMetadataAsync(p);
}

public class MetadataAvailable : IMetadataState
{
  public void Apply(IProductImagesMetadataService metadataService, ProductImagesMetadata p) =>
    metadataService.UpdateMetadataAsync(p);
}
