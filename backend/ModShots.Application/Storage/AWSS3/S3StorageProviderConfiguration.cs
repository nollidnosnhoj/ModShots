using Amazon;
using Amazon.S3;

namespace ModShots.Application.Storage.AWSS3;

public class S3StorageProviderConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool ForcePathStyle { get; set; }

    public AmazonS3Config GetAmazonS3Config()
    {
        var config = new AmazonS3Config();
        if (!string.IsNullOrEmpty(Endpoint))
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(Region);
        }

        if (!string.IsNullOrEmpty(Endpoint))
        {
            config.ServiceURL = Endpoint;
        }
        
        config.ForcePathStyle = ForcePathStyle;
        return config;
    }
}