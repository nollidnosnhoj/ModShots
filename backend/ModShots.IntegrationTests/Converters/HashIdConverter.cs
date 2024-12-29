using ModShots.Application.Common.HashIds;

namespace ModShots.IntegrationTests.Converters;

public class HashIdConverter : WriteOnlyJsonConverter<HashId>
{
    public override void Write(VerifyJsonWriter writer, HashId value)
    {
        writer.WriteValue(value.ToString());
    }
}