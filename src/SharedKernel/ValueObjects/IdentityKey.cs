namespace SharedKernel.ValueObjects;

public sealed record IdentityKey
{
    public string Value { get; }

    private IdentityKey(string value) => Value = value;

    public static IdentityKey Generate()
    {
        string part1 = GeneratePart();
        string part2 = GeneratePart();
        string part3 = GeneratePart();
        return new IdentityKey($"INK-{part1}-{part2}-{part3}");
    }

    public static ErrorOr<IdentityKey> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("IdentityKey.Empty", "Identity key is required.");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^INK-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$"))
        {
            return Error.Validation("IdentityKey.Invalid",
                "Identity key must follow the format INK-XXXX-XXXX-XXXX.");
        }

        return new IdentityKey(value);
    }

    private static string GeneratePart() =>
        new(Enumerable.Range(0, 4)
            .Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[
                System.Security.Cryptography.RandomNumberGenerator.GetInt32(36)])
            .ToArray());

    public override string ToString() => Value;
}
