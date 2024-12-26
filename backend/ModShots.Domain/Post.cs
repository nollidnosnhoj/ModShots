using ModShots.Domain.Common;

namespace ModShots.Domain;

public enum PostStatus
{
    Draft,
    Published
}

public class Post : BaseEntity<int>
{
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

    public static Post Create(List<Media> medias, DateTimeOffset createdAt)
    {
        return new Post
        {
            Title = medias[0].OriginalFileName,
            Description = null,
            Medias = medias,
            CreatedAt = createdAt,
            Status = PostStatus.Draft
        };
    }
}