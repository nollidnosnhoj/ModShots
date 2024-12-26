using ModShots.Application.Features.Posts.Models;
using ModShots.Application.Features.Uploads.Mappers;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts.Mappers;

public static class PostMapper
{
    public static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Severity = post.Severity,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            PublishedAt = post.PublishedAt,
            Medias = post.Medias.Select(UploadMapper.MapToDto).ToList()
        };
    }

    public static IQueryable<PostDto> ProjectToDto(this IQueryable<Post> posts)
    {
        return posts.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Severity = post.Severity,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            PublishedAt = post.PublishedAt,
            Medias = post.Medias
                .Select(media => new UploadDto
                {
                    Id = media.Id,
                    Caption = media.Caption,
                    Height = media.Height,
                    Width = media.Width,
                    MimeType = media.MimeType,
                    FileSize = media.FileSize,
                    Md5 = media.Md5,
                    BlurHash = media.BlurHash,
                })
                .ToList()
        });
    }
}