using ModShots.Domain.Common;
using NJsonSchema;
using NJsonSchema.Generation;

namespace ModShots.Application.Common.OpenAPI;

public class PublicIdSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;

        if (context.ContextualType.OriginalType == typeof(PublicId))
        {
            schema.Type = JsonObjectType.String;
        }
    }
}