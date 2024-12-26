using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application;
using ModShots.Application.Data;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace ModShots.IntegrationTests;

public class TestApplication : FastEndpoints.Testing.AppFixture<Program>
{
    private PostgreSqlContainer _postgresContainer = null!;
    private MinioContainer _minioContainer = null!;

    protected override async Task PreSetupAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("ModShots")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithUsername("minio")
            .WithPassword("minio")
            .Build();
        
        await _postgresContainer.StartAsync();
        await _minioContainer.StartAsync();

        var serviceCollection = new ServiceCollection();
        
        var configurationBuilder = new ConfigurationBuilder();
        AddTestConfiguration(configurationBuilder);
        var configuration = configurationBuilder.Build();
        
        serviceCollection.AddApplicationDbContext(configuration);
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        await using var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder b)
    {
        b.ConfigureAppConfiguration(c => AddTestConfiguration(c));
    }

    private IConfigurationBuilder AddTestConfiguration(IConfigurationBuilder builder)
    {
        return builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:Database", _postgresContainer.GetConnectionString() },
            { "AWS:Region", "us-west-2" },
            { "AWS:Bucket", "modshots-test" },
            { "AWS:Endpoint", _minioContainer.GetConnectionString() },
            { "AWS:AccessKey", "minio" },
            { "AWS:ForcePathStyle", "true" },
        });
    }
}