using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Tests.ValueObjects;

public class TerritoryCodeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("U")]
    [InlineData("USA")]
    public void Constructor_ThrowsArgumentException_WhenCodeIsNotTwoLetters(string? value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new TerritoryCode(value!));

        Assert.Equal("Territory code must be a 2-letter ISO code.", exception.Message);
    }

    [Fact]
    public void Constructor_NormalizesValueToUpperInvariant()
    {
        var code = new TerritoryCode("mx");

        Assert.Equal("MX", code.Value);
    }
}
