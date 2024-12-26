using System.Security.Cryptography;
using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ModShots.Application.Features.Uploads.Helpers;

public static class ImageHelpers
{
    public static string GenerateBlurhash(Image<Rgba32> image)
    {
        using var resizedImage = image.Clone(x => x.Resize(32, 32));
        var blurHash = Blurhasher.Encode(resizedImage, 4, 3);
        return blurHash;
    }

    public static async Task<string> GenerateMd5(Image<Rgba32> image)
    {
        using var memoryStream = new MemoryStream();
        await image.SaveAsync(memoryStream, image.Metadata.DecodedImageFormat ?? JpegFormat.Instance);
        var bytesArray = MD5.HashData(memoryStream.ToArray());
        return Convert.ToHexStringLower(bytesArray);
    }
}