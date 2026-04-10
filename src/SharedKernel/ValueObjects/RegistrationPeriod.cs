namespace SharedKernel.ValueObjects;

public sealed record RegistrationPeriod
{
    public DateOnly OpenDate { get; }
    public DateOnly? CloseDate { get; }

    private RegistrationPeriod(DateOnly openDate, DateOnly? closeDate)
    {
        OpenDate = openDate;
        CloseDate = closeDate;
    }

    public static ErrorOr<RegistrationPeriod> Create(DateOnly openDate, DateOnly? closeDate)
    {
        if (closeDate.HasValue && closeDate.Value <= openDate)
        {
            return Error.Validation("RegistrationPeriod.InvalidRange", "Close Date must be after Open Date");
        }

        return new RegistrationPeriod(openDate, closeDate);
    }


    public bool IsOpen(DateTime now)
    {
        var today = DateOnly.FromDateTime(now);
        return today >= OpenDate && (CloseDate == null || today < CloseDate);
    }
    public bool IsScheduled(DateTime now) => DateOnly.FromDateTime(now) < OpenDate;
    public bool IsPastCloseDate(DateTime now) => CloseDate.HasValue && DateOnly.FromDateTime(now) >= CloseDate.Value;

    public override string ToString() =>
        CloseDate.HasValue
            ? $"{OpenDate:dd/MM/yyy} -> {CloseDate.Value:dd/MM/yyyy}"
            : $"A partir du {OpenDate:dd/MM/yyyy}";

}
