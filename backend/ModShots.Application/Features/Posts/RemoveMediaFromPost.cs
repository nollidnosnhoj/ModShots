using Microsoft.EntityFrameworkCore;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts;

public static class RemoveMediaFromPost
{
    public class Endpoint(ApplicationDbContext dbContext) : FastEndpoints.EndpointWithoutRequest
    {
        public override void Configure()
        {
            Delete("/posts/{PostId}/medias/{MediaId}/");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var postId = Route<HashId>("PostId", isRequired: true);
            var mediaId = Route<Ulid>("MediaId", isRequired: true);
            
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var post = await dbContext.Posts
                    .Include(x => x.Medias)
                    .Where(x => x.Id == postId)
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
                
                var media = post.Medias.SingleOrDefault(x => x.Id == mediaId);
                
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