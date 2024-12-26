using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModShots.Domain;

namespace ModShots.Application.Data.EntityConfigurations;

public class UploadEntityConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FilePath).HasMaxLength(150);
        builder.Property(x => x.MimeType).HasMaxLength(50);
        builder.Property(x => x.Caption).HasMaxLength(255);
        builder.Property(x => x.Md5).HasMaxLength(50);
        builder.Property(x => x.BlurHash).HasMaxLength(50);
    }
}