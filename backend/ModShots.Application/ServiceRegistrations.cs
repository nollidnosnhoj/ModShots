using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application.Data;
using ModShots.Application.Data.Interceptors;
using ModShots.Application.Storage;
using ModShots.Application.Storage.AWSS3;

namespace ModShots.Application;

public static class ServiceRegistrations
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(IApplicationMarker).Assembly));
        services.AddTransient<DispatchDomainEventsInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
            options.AddInterceptors(sp.GetRequiredService<DispatchDomainEventsInterceptor>());
            options.UseSnakeCaseNamingConvention();
        });
        
        return services;
    }

    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddSingleton<S3StorageProvider>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var awsConfiguration = configuration.GetSection("AWS");
            var awsRegion = awsConfiguration.GetValue<string>("Region", "");
            var awsBucket = awsConfiguration.GetValue<string>("Bucket", "");
            var awsAccessKey = awsConfiguration.GetValue<string>("AccessKey", "");
            var awsSecretKey = awsConfiguration.GetValue<string>("SecretKey", "");
            var awsEndpoint = awsConfiguration.GetValue<string>("Endpoint", "");
            var awsForcePathStyle = awsConfiguration.GetValue("ForcePathStyle", false);

            return new S3StorageProvider(config =>
            {
                config.Endpoint = awsEndpoint;
                config.AccessKey = awsAccessKey;
                config.SecretKey = awsSecretKey;
                config.Region = awsRegion;
                config.Bucket = awsBucket;
                config.ForcePathStyle = awsForcePathStyle;
            });
        });
        
        services.AddSingleton<IS3Storage>(sp => sp.GetRequiredService<S3StorageProvider>());
        services.AddSingleton<IStorage>(sp => sp.GetRequiredService<S3StorageProvider>());
        
        return services;
    }
}