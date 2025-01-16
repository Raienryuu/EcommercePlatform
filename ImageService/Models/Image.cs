namespace ImageService.Models;

public class Image
{
	public string? _id {get;init;}
	public required string ContentType { get; init; }

	public required string ContentDisposition { get; init;}

	public required long Length { get; init;}

	public required string Name { get; init;}

	public required string FileName { get; init;}
	public required byte[] Data { get; init; }
}

