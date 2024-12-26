using ModShots.Application.Features.Uploads.Models;

namespace ModShots.Application.Features.Uploads.Mappers;

public static class UploadMapper
{
    public static UploadDto MapToDto(Domain.Media media)
    {
        return new UploadDto
        {
            Id = media.Id,
            Caption = media.Caption,
            MimeType = media.MimeType,
            Width = media.Width,
            Height = media.Height,
            FileSize = media.FileSize,
            BlurHash = media.BlurHash,
            Md5 = media.Md5,
        };
    }
}