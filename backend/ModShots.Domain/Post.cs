using ModShots.Domain.Common;
using NanoidDotNet;

namespace ModShots.Domain;

public enum PostStatus
{
    Draft,
    Published
}

public class Post : BaseEntity<int>, IHasPublicIdentifier
{
    public PublicId PublicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Severity Severity { get; set; }
    public PostStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    
    public List<Media> Medias { get; private set; } = [];

    public void AddMedia(Media media)
    {
        if (Status != PostStatus.Draft)
        {
            throw new InvalidOperationException("Medias can only be added to a draft post");
        }
        
        Medias.Add(media);
    }

    public void RemoveMedia(Media media)
    {
        if (Status != PostStatus.Draft)
        {
            throw new InvalidOperationException("Medias can only be removed from a draft post");
        }
        
        Medias.Remove(media);
    }

    public void Publish(DateTimeOffset publishedAt)
    {
        Status = PostStatus.Published;
        PublishedAt = publishedAt;
    }

    public static Post Create(PublicId publicId, DateTimeOffset createdAt)
    {
        return new Post
        {
            PublicId = publicId,
            Title = $"post_{publicId}",
            Description = null,
            CreatedAt = createdAt,
            Status = PostStatus.Draft
        };
    }

    public static Post Create(DateTimeOffset createdAt)
    {
        var publicId = PublicId.New();
        return Create(publicId, createdAt);
    }
}