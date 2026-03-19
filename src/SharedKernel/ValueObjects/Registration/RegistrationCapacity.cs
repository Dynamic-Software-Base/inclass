namespace SharedKernel.ValueObjects.Registration;

public sealed record RegistrationCapacity
{
    /// <summary>Nombre maximum de dossiers acceptés</summary>
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

    /// <summary>La capacité est-elle atteinte ?</summary>
    public bool IsFull(int currentCount) => currentCount >= MaxSlots;

    /// <summary>Places restantes</summary>
    public int Remaining(int currentCount) => Math.Max(0, MaxSlots - currentCount);

    public override string ToString() => $"{MaxSlots} places";
}
