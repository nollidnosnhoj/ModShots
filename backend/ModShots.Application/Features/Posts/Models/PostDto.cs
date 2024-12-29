using System.Text.Json.Serialization;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.Application.Features.Posts.Models;

public class PostDto
{
    public required PublicId Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required Severity Severity { get; init; }
    public required PostStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? PublishedAt { get; init; }
    
    public required List<UploadDto> Medias { get; init; }
}