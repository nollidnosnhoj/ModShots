using ModShots.Domain;

namespace ModShots.Application.Features.Uploads.Models;

public class UploadDto
{
    public required Ulid Id { get; init; }
    public required string? Caption { get; init; }
    public required string MimeType { get; init; }
    public required long FileSize { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string? Md5 { get; init; }
    public required string? BlurHash { get; init; }
}