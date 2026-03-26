namespace SharedKernel.ValueObjects.Registration;

public sealed record RegistrationCapacity
{
    public int MaxSlots { get; }

    private RegistrationCapacity(int maxSlots) => MaxSlots = maxSlots;

    public static ErrorOr<RegistrationCapacity> Create(int maxSlots)
    {
        if (maxSlots <= 0)
        {
            return Error.Validation("RegistrationCapacity.Invalid",
                "La capacité maximale doit être supérieure à zéro.");
        }


        return new RegistrationCapacity(maxSlots);
    }

    public bool IsFull(int currentCount) => currentCount >= MaxSlots;


    public int Remaining(int currentCount) => Math.Max(0, MaxSlots - currentCount);

    public override string ToString() => $"{MaxSlots} places";
}
