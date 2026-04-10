namespace Domain.Registrations.ValueObjects;

public record DailyProcessingQuota
{
    public int Value { get; init; }
    private DailyProcessingQuota(int value) => Value = value;
    private DailyProcessingQuota() { }

    public static ErrorOr<DailyProcessingQuota> Create(
        int value
    )
    {
        if (value <= 0)
        {
            return Error.Validation(
                "DailyProcessingQuota.Invalid",
                "Le quota journalier doit être supérieur à zéro.");
        }

        return new DailyProcessingQuota(value);
    }

    public DateOnly CalculateProcessingDate(
        DateOnly sessionStartDate,
        int queuePosition,
        DateTime submittedAt,
        TimeOnly dailyCutoff)
    {
        var submittedDate = DateOnly.FromDateTime(submittedAt);
        var submittedTime = TimeOnly.FromDateTime(submittedAt);

        // Effective base: never assign a date in the past
        DateOnly effectiveBase = sessionStartDate > submittedDate ? sessionStartDate : submittedDate;

        // If submitted after cutoff today, next available slot starts tomorrow
        if (submittedTime > dailyCutoff)
        {
            effectiveBase = effectiveBase.AddDays(1);
        }

        int daysOffset = (queuePosition - 1) / Value;
        return effectiveBase.AddDays(daysOffset);
    }

    public override string ToString() => $"{Value} dossiers/jour";
}
