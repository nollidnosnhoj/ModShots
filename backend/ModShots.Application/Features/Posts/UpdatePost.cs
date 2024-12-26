using Microsoft.EntityFrameworkCore;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts.Mappers;
using ModShots.Application.Features.Posts.Models;
using ModShots.Domain;

namespace ModShots.Application.Features.Posts;

public static class UpdatePost
{
    public class Request
    {
        public required string Title { get; init; }
        public required string? Description { get; init; }
        public required Severity Severity { get; init; }
    }

    public class Endpoint(ApplicationDbContext dbContext) : FastEndpoints.Endpoint<Request, PostDto>
    {
        public override void Configure()
        {
            Patch("/posts/{PostId}/");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var postId = Route<HashId>("PostId", isRequired: true);
            
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var post = await dbContext.Posts
                    .Where(x => x.Id == postId)
                    .SingleOrDefaultAsync(ct);

                if (post is null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }

                post.Title = req.Title;
                post.Description = req.Description;
                post.Severity = req.Severity;

                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                await SendOkAsync(PostMapper.MapToDto(post), ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}