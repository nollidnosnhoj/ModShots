using Amazon.S3;

namespace ModShots.Application.Storage.AWSS3;

public interface IS3Storage
{
    IAmazonS3 Client { get; }
    Task<string> CreateUploadUrlAsync(string key, string mimeType, TimeSpan expirationTimeSpan, CancellationToken cancellationToken = default);
}