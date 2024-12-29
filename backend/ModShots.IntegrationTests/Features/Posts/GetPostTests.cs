using System.Net;
using Argon;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application.Data;
using ModShots.Application.Features.Posts;
using ModShots.Application.Features.Posts.Models;
using ModShots.Application.Features.Uploads.Models;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.IntegrationTests.Features.Posts;

public class GetPostTests : TestBase<TestApplication>
{
    private readonly TestApplication _app;

    public GetPostTests(TestApplication app)
    {
        _app = app;
        VerifierSettings.ScrubMembers(typeof(PostDto), nameof(PostDto.Id));
        VerifierSettings.ScrubMembers(typeof(UploadDto), nameof(UploadDto.Id));
        VerifierSettings.AddExtraSettings(s =>
        {
            s.DefaultValueHandling = DefaultValueHandling.Populate;
        });
    }
    
    [Fact]
    public async Task SuccessfullyGetPost()
    {
        Post post;
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
            post = Post.Create(timeProvider.GetUtcNow());
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
        }

        var (response, returnedPost) = await _app.Client
            .GETAsync<GetPost.Endpoint, GetPost.Request, PostDto>(
                new GetPost.Request
                {
                    PostId = post.PublicId
                });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedPost.Id.Should().Be(post.PublicId);
        returnedPost.Title.Should().Be(post.Title);
    }
}