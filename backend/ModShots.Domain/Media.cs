using ModShots.Domain.Common;

namespace ModShots.Domain;

public class Media : BaseEntity<Ulid>
{
    public string? Caption { get; set; }
    public string FilePath { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string? Md5 { get; set; }
    public string? BlurHash { get; set; }
    public bool IsComplete { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    public int? PostId { get; private set; }
    
    public void SetDimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void Complete()
    {
        if (Width <= 0 || Height <= 0)
        {
            throw new InvalidOperationException("Media width and height must be set before processing");
        }
        if (string.IsNullOrEmpty(Md5))
        {
            throw new InvalidOperationException("Md5 hash must be set before processing");
        }
        if (string.IsNullOrEmpty(BlurHash))
        {
            throw new InvalidOperationException("Blur hash must be set before processing");
        }
        
        IsComplete = true;
    }

    public static Media Create(string fileName, string mimeType, long fileSize, DateTimeOffset createdAt)
    {
        var key = Ulid.NewUlid();
        var fileExtension = Path.GetExtension(fileName);
        var filePath = key + fileExtension;

        return new Media
        {
            Id = key,
            FilePath = filePath,
            OriginalFileName = fileName,
            MimeType = mimeType,
            FileSize = fileSize,
            Width = 0,
            Height = 0,
            CreatedAt = createdAt,
        };
    }

    public static Media Create(int postId, string fileName, string mimeType, long fileSize, DateTimeOffset createdAt)
    {
        var media = Create(fileName, mimeType, fileSize, createdAt);
        media.PostId = postId;
        return media;
    }
}