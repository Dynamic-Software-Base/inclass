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
        int daysOffset = (queuePosition - 1) / Value;
        DateOnly assignedDate = sessionStartDate.AddDays(daysOffset);
        var submittedDate = DateOnly.FromDateTime(submittedAt);
        var submittedTime = TimeOnly.FromDateTime(submittedAt);

        if (submittedDate == assignedDate && submittedTime > dailyCutoff)
        {
            assignedDate = assignedDate.AddDays(1);
        }

        return assignedDate;
    }

    public override string ToString() => $"{Value} dossiers/jour";
}
