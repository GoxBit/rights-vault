using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Tests.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Constructor_WithValidDates_SetsStartAndEnd()
    {
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 1, 31);

        var range = new DateRange(start, end);

        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void Constructor_WhenEndIsNotAfterStart_ThrowsArgumentException(
        int startDay,
        int endDay)
    {
        var start = new DateOnly(2026, 1, startDay);
        var end = new DateOnly(2026, 1, endDay);

        var exception = Assert.Throws<ArgumentException>(
            () => new DateRange(start, end));

        Assert.Equal("End must be after Start.", exception.Message);
    }

    [Fact]
    public void TwoRanges_WithSameDates_AreEqual()
    {
        var first = new DateRange(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 31));
        var second = new DateRange(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 31));

        Assert.Equal(first, second);
    }

    [Fact]
    public void OverlapsWith_WhenRangesIntersect_ReturnsTrue()
    {
        var first = new DateRange(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 20));
        var second = new DateRange(
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 1, 31));

        Assert.True(first.OverlapsWith(second));
        Assert.True(second.OverlapsWith(first));
    }

    [Fact]
    public void OverlapsWith_WhenRangesOnlyTouch_ReturnsFalse()
    {
        var first = new DateRange(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 15));
        var second = new DateRange(
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 31));

        Assert.False(first.OverlapsWith(second));
        Assert.False(second.OverlapsWith(first));
    }

    [Fact]
    public void OverlapsWith_WhenRangesAreSeparated_ReturnsFalse()
    {
        var first = new DateRange(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 10));
        var second = new DateRange(
            new DateOnly(2026, 1, 20),
            new DateOnly(2026, 1, 31));

        Assert.False(first.OverlapsWith(second));
        Assert.False(second.OverlapsWith(first));
    }
}
