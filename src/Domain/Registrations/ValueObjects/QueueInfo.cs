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
}
