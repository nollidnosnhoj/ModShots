using FastEndpoints;
using FastEndpoints.Swagger;
using ModShots.Application;
using ModShots.Application.Common.HashIds;
using ModShots.Application.Data.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblies(typeof(IApplicationMarker).Assembly);
    })
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddApplicationDbContext(builder.Configuration)
    .AddStorage(builder.Configuration)
    .AddFastEndpoints()
    .AddSwaggerDocument(opts =>
    {
        opts.SchemaSettings.SchemaProcessors.Add(new HashIdSchemaProcessor());
    });

var app = builder.Build();

app.UseFastEndpoints(c =>
    {
        c.Serializer.Options.Converters.Add(new HashIdJsonConverter());
    })
    .UseSwaggerGen();

app.Run();

public partial class Program { }