namespace SharedKernel.ValueObjects;

public sealed record RegistrationPeriod
{
    public DateTime OpenDate { get; }
    public DateTime? CloseDate { get; }

    private RegistrationPeriod(DateTime openDate, DateTime? closeDate)
    {
        OpenDate = openDate;
        CloseDate = closeDate;
    }

    public static ErrorOr<RegistrationPeriod> Create(DateTime openDate, DateTime? closeDate)
    {
        if (closeDate.HasValue  && closeDate.Value <= openDate)
        {
            return Error.Validation("RegistrationPeriod.InvalidRange","Close Date must be after Open Date");
        }

        return new RegistrationPeriod(openDate, closeDate);
    }

    public bool IsOpen(DateTime now) => now >= OpenDate && (CloseDate == null || now < CloseDate);
    public bool IsScheduled(DateTime now) => now < OpenDate;
    public bool IsPastCloseDate(DateTime now) => CloseDate.HasValue && now >= CloseDate.Value;

    public override string ToString() =>
        CloseDate.HasValue
            ? $"{OpenDate:dd/MM/yyy} -> {CloseDate.Value:dd/MM/yyyy}"
            : $"A partir du {OpenDate:dd/MM/yyyy}";

}
