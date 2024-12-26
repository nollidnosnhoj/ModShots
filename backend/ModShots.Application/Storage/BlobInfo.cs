namespace ModShots.Application.Storage;

public class BlobInfo
{
    public required string FilePath { get; init; }
    public required long? FileSize { get; init; }
    public required string MimeType { get; init; }
}