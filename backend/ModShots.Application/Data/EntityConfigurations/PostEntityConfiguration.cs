using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModShots.Domain;
using ModShots.Domain.Common;

namespace ModShots.Application.Data.EntityConfigurations;

public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();
        builder.Property(x => x.PublicId).HasMaxLength(PublicId.Size);

        builder.HasIndex(x => x.PublicId).IsUnique();
    }
}