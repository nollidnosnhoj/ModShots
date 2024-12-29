using NanoidDotNet;

namespace ModShots.Domain.Common;

public readonly struct PublicId : IEquatable<PublicId>
{
    public const int Size = 12;
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private readonly string _value;

    private PublicId(string value)
    {
        _value = value;
    }
    
    public static implicit operator string(PublicId publicId) => publicId._value;
    public static implicit operator PublicId(string value) => new(value);
    
    public override string ToString() => _value;
    
    public bool Equals(PublicId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is PublicId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(PublicId left, PublicId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PublicId left, PublicId right)
    {
        return !(left == right);
    }
    
    public static bool TryParse(string? input, out PublicId output)
    {
        output = default;
        if (string.IsNullOrWhiteSpace(input)) return false;
        
        output = new PublicId(input);
        return true;
    }

    public static PublicId New()
    {
        var publicId = Nanoid.Generate(size: Size, alphabet: Alphabet);
        return publicId;
    }
}

public interface IHasPublicIdentifier
{
    public PublicId PublicId { get; }
}