using FastEndpoints;
using ModShots.Application;
using ModShots.Application.Common.JsonConverters;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblies(typeof(IApplicationMarker).Assembly);
    })
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddApplicationDbContext(builder.Configuration)
    .AddStorage()
    .AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Serializer.Options.Converters.Add(new PublicIdJsonConverter());
});

app.Run();