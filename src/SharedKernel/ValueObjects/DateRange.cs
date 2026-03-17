namespace SharedKernel.ValueObjects;

public sealed record DateRange
{
    public DateTime StartsAtUtc  { get; init; }
    public DateTime EndsAtUtc    { get; init; }

    private DateRange(DateTime startsAtUtc, DateTime endsAtUtc)
    {
        StartsAtUtc = startsAtUtc;
        EndsAtUtc   = endsAtUtc;
    }

    private DateRange(){}

    public static ErrorOr<DateRange> Create(
        DateTime startsAtUtc,
        DateTime endsAtUtc
    )
    {
        if (endsAtUtc <= startsAtUtc)
        {
            return DomainErrors.DateRangeErrors.EndsMussBeAfterStart;
        }

        return new DateRange(startsAtUtc, endsAtUtc);
    }
}
