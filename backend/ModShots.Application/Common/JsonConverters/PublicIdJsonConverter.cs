using System.Text.Json;
using System.Text.Json.Serialization;
using ModShots.Domain.Common;

namespace ModShots.Application.Common.JsonConverters;

public class PublicIdJsonConverter : JsonConverter<PublicId>
{
    public override PublicId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("PublicId type must be a string");
        }
        
        var value = reader.GetString();
        
        if (PublicId.TryParse(value, out var publicId))
        {
            return publicId;
        }
        
        throw new JsonException("Unable to parse PublicId");
    }

    public override void Write(Utf8JsonWriter writer, PublicId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}