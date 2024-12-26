using Microsoft.EntityFrameworkCore;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts.Mappers;
using ModShots.Application.Features.Posts.Models;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts;

public static class PublishPost
{
    public class Endpoint(ApplicationDbContext dbContext, TimeProvider timeProvider) : FastEndpoints.EndpointWithoutRequest<PostDto>
    {
        public override void Configure()
        {
            Patch("/posts/{PostId}/publish/");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var postId = Route<HashId>("PostId", isRequired: true);
            
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