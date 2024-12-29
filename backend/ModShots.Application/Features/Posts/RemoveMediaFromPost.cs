using Microsoft.EntityFrameworkCore;
using ModShots.Application.Data;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.Application.Features.Posts;

public static class RemoveMediaFromPost
{
    public class Request
    {
        public required PublicId PostId { get; init; }
        public required Ulid MediaId { get; init; }
    }
    
    public class Endpoint(ApplicationDbContext dbContext) : FastEndpoints.Endpoint<Request>
    {
        public override void Configure()
        {
            Delete("/posts/{PostId}/medias/{MediaId}/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var post = await dbContext.Posts
                    .Include(x => x.Medias)
                    .Where(x => x.PublicId == req.PostId)
                    .SingleOrDefaultAsync(ct);
                
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
                
                var media = post.Medias.SingleOrDefault(x => x.Id == req.MediaId);
                
                if (media is null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }
                
                post.RemoveMedia(media);
                
                await dbContext.SaveChangesAsync(ct);
                
                await transaction.CommitAsync(ct);
                
                await SendNoContentAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}