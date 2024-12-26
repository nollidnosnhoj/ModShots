
using ModShots.Application.Data;
using ModShots.Application.Features.Uploads.Helpers;
using ModShots.Application.Features.Uploads.Mappers;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Application.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ModShots.Application.Features.Uploads;

public static class CompleteUpload
{
    public class Request
    {
        public required string Id { get; init; }
    }

    public class Response
    {
        public required UploadDto Upload { get; init; }
    }

    public class Endpoint(ApplicationDbContext dbContext, IStorage blobStorage) : FastEndpoints.Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Patch("/uploads/{Id}/complete/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            if (!Ulid.TryParse(req.Id, out var mediaId))
            {
                ThrowError(x => x.Id, "Invalid ID");
                return;
            }

            var media = await dbContext.Medias.FindAsync([mediaId], ct);
            
            if (media is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (media.IsComplete)
            {
                ThrowError(x => x.Id, "Media is already completed.");
                return;
            }

            using var memoryStream = new MemoryStream();
            await using (var stream = await blobStorage.OpenFileAsync(media.FilePath, ct))
            {
                if (stream is null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }

                await stream.CopyToAsync(memoryStream, ct);
            }

            using var image = await Image.LoadAsync<Rgba32>(memoryStream, ct);
            var width = image.Width;
            var height = image.Height;
            var blurHash = ImageHelpers.GenerateBlurhash(image);
            var md5Hash = await ImageHelpers.GenerateMd5(image);

            media.SetDimensions(width, height);
            media.BlurHash = blurHash;
            media.Md5 = md5Hash;
            media.Complete();
                
            await dbContext.SaveChangesAsync(ct);
                
            await SendOkAsync(new Response { Upload = UploadMapper.MapToDto(media) }, ct);
        }
    }
}