using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts.Mappers;
using ModShots.Application.Features.Posts.Models;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Application.Storage;
using ModShots.Application.Storage.AWSS3;
using ModShots.Domain;
using NanoidDotNet;

namespace ModShots.Application.Features.Posts;

public static class CreatePost
{
    public class Endpoint(
        ApplicationDbContext dbContext, 
        TimeProvider timeProvider,
        ILogger<Endpoint> logger)
        : FastEndpoints.EndpointWithoutRequest<PostDto>
    {
        public override void Configure()
        {
            Post("/posts/");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var postPublicId = await GeneratePublicId(ct);
                var post = Domain.Post.Create(postPublicId, timeProvider.GetUtcNow());
                await dbContext.Posts.AddAsync(post, ct);
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

        private async Task<string> GeneratePublicId(CancellationToken ct = default)
        {
            var iteration = 0;
            while (true)
            {
                if (iteration == 2)
                {
                    logger.LogWarning("Encountered 2 collisions when generating public id");
                }
                
                var publicId = await Nanoid.GenerateAsync(size: 12);
                var exists = await dbContext.Posts.AnyAsync(x => x.PublicId == publicId, ct);
                
                if (!exists) return publicId;
                
                iteration++;
            }
        }
    }
}