namespace RightsVault.Domain.ValueObjects;

public sealed record DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End must be after Start.");
        }

        Start = start;
        End = end;
    }

    public bool OverlapsWith(DateRange other) =>
        Start < other.End && End > other.Start;
}
