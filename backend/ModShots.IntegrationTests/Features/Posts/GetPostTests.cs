using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts;
using ModShots.Application.Features.Posts.Models;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Domain;
using ModShots.IntegrationTests.Converters;

namespace ModShots.IntegrationTests.Features.Posts;

public class GetPostTests : TestBase<TestApplication>
{
    private readonly TestApplication _app;

    public GetPostTests(TestApplication app)
    {
        _app = app;
        VerifierSettings.AddExtraSettings(s => s.Converters.Add(new HashIdConverter()));
    }
    
    [Fact]
    public async Task SuccessfullyGetPost()
    {
        HashId postId;
        using (var scope = _app.Server.Services.CreateScope())
        {
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var medias = Enumerable.Range(0, 2)
                .Select(_ => Media.Create(
                    "test.jpg", 
                    "image/jpeg", 
                    100_000_000, 
                    timeProvider.GetUtcNow()))
                .ToList();
            var post = Post.Create(medias, timeProvider.GetUtcNow());
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            postId = post.Id;
        }

        var (response, returnedPost) = await _app.Client
            .GETAsync<GetPost.Endpoint, GetPost.Request, PostDto>(
                new GetPost.Request
                {
                    PostId = postId
                });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await Verify(returnedPost)
            .ScrubMembers(typeof(PostDto), nameof(PostDto.Id))
            .ScrubMembers(typeof(UploadDto), nameof(UploadDto.Id));
    }
}