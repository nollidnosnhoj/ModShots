using NJsonSchema;
using NJsonSchema.Generation;

namespace ModShots.Application.Common.HashIds;

public class HashIdSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;

        if (context.ContextualType.OriginalType == typeof(HashId))
        {
            schema.Type = JsonObjectType.String;
        }
    }
}