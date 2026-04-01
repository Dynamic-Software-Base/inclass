namespace Domain.Registrations.ValueObjects;

public sealed record QueueInfo
{
    public int Position { get; init; }
    public DateOnly ProcessingDate { get; init; }
    internal QueueInfo(int position, DateOnly processingDate)
    {
        Position = position;
        ProcessingDate = processingDate;
    }


    public static ErrorOr<QueueInfo> Create(int position, DateOnly processingDate)
    {
        if (position <= 0)
        {
            return Error.Validation(
                "QueueInfo.InvalidPosition",
                "La position dans la file doit être supérieure à zéro.");
        }

        return new QueueInfo(position, processingDate);
    }
    public static ErrorOr<QueueInfo> CreateRescheduled(
        int position,
        DateOnly originalProcessingDate,
        DateOnly calculatedNewDate)
    {
        if (position <= 0)
        {
            return Error.Validation(
                "QueueInfo.InvalidPosition",
                "La position dans la file doit être supérieure à zéro.");
        }

        DateOnly cap = originalProcessingDate.AddDays(2);
        DateOnly finalDate = calculatedNewDate > cap ? cap : calculatedNewDate;

        return new QueueInfo(position, finalDate);
    }
}
