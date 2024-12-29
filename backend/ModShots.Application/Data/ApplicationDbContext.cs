using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModShots.Application.Data.Interceptors;
using ModShots.Application.Data.ValueConverters;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.Application.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Media> Medias => Set<Media>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>();
        
        configurationBuilder
            .Properties<PublicId>()
            .HaveConversion<PublicIdToStringConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}