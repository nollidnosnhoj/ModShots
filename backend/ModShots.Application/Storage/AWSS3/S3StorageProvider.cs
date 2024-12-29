using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace ModShots.Application.Storage.AWSS3;

public class S3StorageProvider : IStorage, IS3Storage
{
    private readonly string _bucketName;
    private bool _isInitialized;

    public S3StorageProvider(Action<S3StorageProviderConfiguration> configure)
    {
        var config = new S3StorageProviderConfiguration();
        configure(config);
        _bucketName = config.Bucket;
        Client = new AmazonS3Client(config.AccessKey, config.SecretKey, config.GetAmazonS3Config());
    }
    
    public async Task<Stream?> OpenFileAsync(string key, CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
        };
        
        var response = await client.GetObjectAsync(request, cancellationToken);

        if ((int) response.HttpStatusCode >= 400)
        {
            return null;
        }

        return response.ResponseStream;
    }

    public async Task<BlobInfo?> GetBlobMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);
        
        var request = new GetObjectMetadataRequest
        {
            BucketName = _bucketName,
            Key = key,
        };
        
        var response = await client.GetObjectMetadataAsync(request, cancellationToken);

        if ((int) response.HttpStatusCode >= 400)
        {
            return null;
        }

        return new BlobInfo
        {
            FilePath = key,
            FileSize = response.ContentLength,
            MimeType = response.Headers.ContentType,
        };
    }

    public async Task<string> CreateUploadUrlAsync(string key, string mimeType, TimeSpan expirationTimeSpan,
        CancellationToken cancellationToken = default)
    {
        var client = await GetClientAsync(cancellationToken);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            ContentType = mimeType,
            Expires = DateTime.UtcNow.Add(expirationTimeSpan),
            Verb = HttpVerb.PUT
        };
        
        var response = await client.GetPreSignedURLAsync(request);
        return response;
    }

    public IAmazonS3 Client { get; }

    protected async Task<IAmazonS3> GetClientAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized) return Client;

        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(Client, _bucketName);

        if (!bucketExists)
        {
            var request = new PutBucketRequest{BucketName = _bucketName};
            await Client.PutBucketAsync(request, cancellationToken);
        }
        
        _isInitialized = true;
        return Client;
    }
}