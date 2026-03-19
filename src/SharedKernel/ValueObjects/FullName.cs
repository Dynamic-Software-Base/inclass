namespace SharedKernel.ValueObjects;

public record FullName
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public ErrorOr<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return DomainErrors.Required(nameof(firstName) + nameof(lastName));
        }

        return new FullName(firstName, lastName);
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}
