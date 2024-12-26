using ModShots.Application.Data;
using ModShots.Application.Features.Uploads.Mappers;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Application.Storage;
using ModShots.Application.Storage.AWSS3;
using ModShots.Domain;

namespace ModShots.Application.Features.Uploads;

public static class PrepareUpload
{
    public class Request
    {
        public required string FileName { get; init; }
        public required string MimeType { get; init; }
        public required long FileSize { get; init; }
    }

    public class Response
    {
        public required string UploadUrl { get; init; }
        public required UploadDto Upload { get; init; }
    }

    public class Endpoint(ApplicationDbContext dbContext, IS3Storage blobStorage, TimeProvider timeProvider) : FastEndpoints.Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("/uploads/");
        }
        
        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var media = Media.Create(req.FileName, req.MimeType, req.FileSize, timeProvider.GetUtcNow());
            await dbContext.Medias.AddAsync(media, ct);
            await dbContext.SaveChangesAsync(ct);
            var url = await blobStorage.CreateUploadUrlAsync(media.FilePath, media.MimeType, TimeSpan.FromHours(1), ct);
            await SendOkAsync(new Response { Upload = UploadMapper.MapToDto(media), UploadUrl = url }, ct);
        }
    }
}