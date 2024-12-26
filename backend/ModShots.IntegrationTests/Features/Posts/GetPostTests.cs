using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts;
using ModShots.Application.Features.Posts.Models;
using ModShots.Domain;

namespace ModShots.IntegrationTests.Features.Posts;

public class GetPostTests(TestApplication app) : TestBase<TestApplication>
{
    [Fact]
    public async Task SuccessfullyGetPost()
    {
        using (var scope = app.Server.Services.CreateScope())
        {
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var medias = Enumerable.Range(0, 2)
                .Select(_ => Media.Create(
                    Fake.System.FileName(".jpg"), 
                    "image/jpeg", 
                    100_000_000, 
                    timeProvider.GetUtcNow()))
                .ToList();
            var post = Post.Create(medias, timeProvider.GetUtcNow());
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
        }

        var (response, returnedPost) = await app.Client.GETAsync<GetPost.Endpoint, PostDto>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await Verify(returnedPost);
    }
}