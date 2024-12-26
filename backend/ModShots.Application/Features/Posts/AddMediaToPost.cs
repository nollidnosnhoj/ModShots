using Microsoft.EntityFrameworkCore;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Application.Storage;
using ModShots.Application.Storage.AWSS3;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts;

public static class AddMediaToPost
{
    public class Request
    {
        public required string FileName { get; init; }
        public required string MimeType { get; init; }
        public required long FileSize { get; init; }
    }

    public class Endpoint(ApplicationDbContext dbContext, IS3Storage blobStorage, TimeProvider timeProvider) 
        : FastEndpoints.Endpoint<Request, UploadMediaRequest>
    {
        public override void Configure()
        {
            Post("/posts/{PostId}/medias/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var postId = Route<HashId>("PostId", isRequired: true);
            
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var post = await dbContext.Posts
                    .Include(x => x.Medias)
                    .Where(x => x.Id == postId)
                    .FirstOrDefaultAsync(ct);
                
                if (post is null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }
                
                if (post.Status != PostStatus.Draft)
                {
                    await SendForbiddenAsync(ct);
                    return;
                }
                
                var media = Media.Create(post.Id, req.FileName, req.MimeType, req.FileSize, timeProvider.GetUtcNow());
                post.AddMedia(media);
                
                await dbContext.SaveChangesAsync(ct);

                var uploadResponse = await CreateUploadResponse(media, blobStorage, ct);
            
                await transaction.CommitAsync(ct);
            
                await SendOkAsync(uploadResponse, ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }

    public static async Task<UploadMediaRequest> CreateUploadResponse(Media media, IS3Storage blobStorage, CancellationToken ct = default)
    {
        var url = await blobStorage.CreateUploadUrlAsync(media.FilePath, media.MimeType, TimeSpan.FromHours(1), ct);
        return new UploadMediaRequest { Id = media.Id, UploadUrl = url };
    }
}