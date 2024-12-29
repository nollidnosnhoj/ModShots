
using HashidsNet;

namespace ModShots.Application.Common.HashIds;

[Serializable]
public readonly record struct HashId
{
    private static readonly Hashids _hid = new(salt: "salt", minHashLength: 12);

    private readonly int _int;
    private readonly string _hash;
    
    internal HashId(int intId)
    {
        _int = intId;
        _hash = _hid.Encode(_int);
    }
    
    internal HashId(string hashId)
    {
        _hash = hashId;
        _int = _hid.DecodeSingle(hashId);
    }
    
    public static implicit operator int(HashId hashId) => hashId._int;
    public static implicit operator HashId(int intId) => new(intId);
    public static implicit operator HashId(string hashId) => new(hashId);
    public static implicit operator string(HashId hashId) => hashId._hash;
    
    public override string ToString() => _hash;

    public static bool TryParse(string input, out HashId output)
    {
        output = default;
        if (string.IsNullOrWhiteSpace(input)) return false;
        
        output = new HashId(input);
        return true;
    }
}