#nullable disable
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModShots.Domain.Common;

namespace ModShots.Application.Data.ValueConverters;

public class UlidToStringConverter : ValueConverter<Ulid, string>
{
    private static readonly ConverterMappingHints DefaultHints = new(size: 26);

    public UlidToStringConverter() : this(null)
    {
    }

    public UlidToStringConverter(ConverterMappingHints mappingHints = null)
        : base(
            convertToProviderExpression: x => x.ToString(),
            convertFromProviderExpression: x => Ulid.Parse(x),
            mappingHints: DefaultHints.With(mappingHints))
    {
    }
}

public class PublicIdToStringConverter : ValueConverter<PublicId, string>
{
    private static readonly ConverterMappingHints DefaultHints = new(size: PublicId.Size);
    
    public PublicIdToStringConverter() : this(null)
    {
    }

    public PublicIdToStringConverter(ConverterMappingHints mappingHints = null)
        : base(
            convertToProviderExpression: x => x.ToString(),
            convertFromProviderExpression: x => x,
            mappingHints: DefaultHints.With(mappingHints))
    {
    }
}