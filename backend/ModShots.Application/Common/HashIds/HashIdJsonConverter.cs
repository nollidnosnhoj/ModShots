using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModShots.Application.Common.HashIds;

public class HashIdJsonConverter : JsonConverter<HashId>
{
    public override HashId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string");
        }
        
        var value = reader.GetString();
        
        if (HashId.TryParse(value, out var hashId))
        {
            return (HashId) hashId!;
        }
        
        throw new JsonException("Unable to parse HashId");
    }

    public override void Write(Utf8JsonWriter writer, HashId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}