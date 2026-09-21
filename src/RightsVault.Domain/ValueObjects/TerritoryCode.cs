namespace RightsVault.Domain.ValueObjects;

public sealed record TerritoryCode
{
    public string Value { get; }

    public TerritoryCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 2)
            throw new ArgumentException("Territory code must be a 2-letter ISO code.");

        Value = value.ToUpperInvariant();
    }
}
