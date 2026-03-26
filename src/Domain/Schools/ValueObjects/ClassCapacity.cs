namespace Domain.Schools.ValueObjects;

public sealed record ClassCapacity
{
    public int MaxStudents { get; }

    private ClassCapacity(int maxStudents) => MaxStudents = maxStudents;

    public static ErrorOr<ClassCapacity> Create(int maxStudents)
    {
        if (maxStudents <= 0)
        {
            return Error.Validation(
                "ClassCapacity.Invalid",
                "La capacité de la classe doit être supérieure à zéro.");
        }

        if (maxStudents > 60)
        {
            return Error.Validation(
                "ClassCapacity.TooLarge",
                "La capacité de la classe ne peut pas dépasser 60 élèves.");
        }

        return new ClassCapacity(maxStudents);
    }

    public bool IsFull(int currentCount) => currentCount >= MaxStudents;
    public int Remaining(int currentCount) => Math.Max(0, MaxStudents - currentCount);

    public override string ToString() => $"{MaxStudents} élèves max";
}
