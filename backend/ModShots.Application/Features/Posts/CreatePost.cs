using ModShots.Application.Data;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Application.Storage;
using ModShots.Application.Storage.AWSS3;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts;

public static class CreatePost
{
    public class Request
    {
        public required List<Media> Medias { get; init; }

        public class Media
        {
            public required string FileName { get; init; }
            public required string MimeType { get; init; }
            public required long FileSize { get; init; }
        }
    }

    public class Response
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string? Description { get; init; }
        public required Severity Severity { get; init; }
        public required PostStatus Status { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        
        public required List<UploadMediaRequest> Medias { get; init; }
    }

    public class Endpoint(ApplicationDbContext dbContext, IS3Storage blobStorage, TimeProvider timeProvider) 
        : FastEndpoints.Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("/posts/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var medias = req.Medias
                    .Select(x => Media.Create(x.FileName, x.MimeType, x.FileSize, timeProvider.GetUtcNow()))
                    .ToList();

                var post = Domain.Post.Create(medias, timeProvider.GetUtcNow());
                await dbContext.Posts.AddAsync(post, ct);
                await dbContext.SaveChangesAsync(ct);

                List<UploadMediaRequest> uploadRequests = [];
                foreach (var media in medias)
                {
                    var uploadResponse = await AddMediaToPost.CreateUploadResponse(media, blobStorage, ct);
                    uploadRequests.Add(uploadResponse);
                }
                
                await transaction.CommitAsync(ct);

                await SendOkAsync(new Response
                {
                    Id = post.Id,
                    Title = post.Title,
                    Description = post.Description,
                    Severity = post.Severity,
                    Status = post.Status,
                    CreatedAt = post.CreatedAt,
                    Medias = uploadRequests
                }, ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}