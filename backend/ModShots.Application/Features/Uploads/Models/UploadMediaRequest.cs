namespace ModShots.Application.Features.Uploads.Models;

public class UploadMediaRequest
{
    public required Ulid Id { get; init; }
    public required string UploadUrl { get; init; }
}