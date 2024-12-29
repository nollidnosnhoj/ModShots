using ModShots.Domain.Common;

namespace ModShots.IntegrationTests.Converters;

public class PublicIdJsonConverter : WriteOnlyJsonConverter<PublicId>
{
    public override void Write(VerifyJsonWriter writer, PublicId value)
    {
        writer.WriteValue(value.ToString());
    }
}