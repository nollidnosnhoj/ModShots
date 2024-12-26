using System.Net;

namespace ModShots.Application.Storage;

public interface IStorage
{
    Task<Stream?> OpenFileAsync(string key, CancellationToken cancellationToken = default);
    Task<BlobInfo?> GetBlobMetadataAsync(string key, CancellationToken cancellationToken = default);
}