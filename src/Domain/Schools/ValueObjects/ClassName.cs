namespace Domain.Schools.ValueObjects;

public sealed record ClassName
{
    public string Value { get; }

    private ClassName(string value) => Value = value;

    public static ErrorOr<ClassName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation(
                "ClassName.Required",
                "Le nom de la classe est obligatoire.");
        }

        if (value.Length > 20)
        {
            return Error.Validation(
                "ClassName.TooLong",
                "Le nom de la classe ne peut pas dépasser 20 caractères.");
        }

        return new ClassName(value.Trim());
    }

    public override string ToString() => Value;
}
