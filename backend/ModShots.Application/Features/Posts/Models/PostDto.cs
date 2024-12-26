using System.Text.Json.Serialization;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts.Models;

public class PostDto
{
    // // ReSharper disable once InconsistentNaming
    // [JsonIgnore] public required int _Id { get; init; }
    // public HashId Id => new(_Id);
    public required HashId Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required Severity Severity { get; init; }
    public required PostStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? PublishedAt { get; init; }
    
    public required List<UploadDto> Medias { get; init; }
}