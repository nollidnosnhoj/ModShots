using Microsoft.EntityFrameworkCore;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts.Mappers;
using ModShots.Application.Features.Posts.Models;

namespace ModShots.Application.Features.Posts;

public static class GetPost
{
    public class Endpoint(ApplicationDbContext dbContext) : FastEndpoints.EndpointWithoutRequest<PostDto>
    {
        public override void Configure()
        {
            Get("/posts/{PostId}/");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var postId = Route<HashId>("PostId", isRequired: true);

            var post = await dbContext.Posts
                .Where(p => p.Id == postId)
                .ProjectToDto()
                .SingleOrDefaultAsync(ct);

            if (post is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await SendOkAsync(post, ct);
        }
    }
}