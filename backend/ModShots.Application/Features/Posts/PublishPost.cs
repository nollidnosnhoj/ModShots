using Microsoft.EntityFrameworkCore;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts.Mappers;
using ModShots.Application.Features.Posts.Models;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.Application.Features.Posts;

public static class PublishPost
{
    public class Request
    {
        public required PublicId PostId { get; init; }
    }
    
    public class Endpoint(ApplicationDbContext dbContext, TimeProvider timeProvider) : FastEndpoints.Endpoint<Request, PostDto>
    {
        public override void Configure()
        {
            Patch("/posts/{PostId}/publish/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
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

            foreach (var media in post.Medias)
            {
                if (media.IsComplete) continue;
                ThrowError("Not all medias are complete");
                return;
            }
            
            post.Publish(timeProvider.GetUtcNow());
            await dbContext.SaveChangesAsync(ct);
            
            await SendOkAsync(PostMapper.MapToDto(post), ct);
        }
    }
}