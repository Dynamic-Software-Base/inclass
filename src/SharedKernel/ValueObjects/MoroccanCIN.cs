using System.Text.RegularExpressions;

namespace SharedKernel.ValueObjects;

/// <summary>
/// Carte Nationale d'Identité marocaine.
/// Format : 1-2 lettres + 5-6 chiffres (ex: A12345, BE123456)
/// Used specifically for ParentTuteur — adults always have a CIN.
/// </summary>
public sealed record MoroccanCin
{
    private static readonly Regex CinRegex = new(
        @"^[A-Za-z]{1,2}\d{5,6}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public string Value { get; }

    private MoroccanCin(string value) => Value = value;

    public static ErrorOr<MoroccanCin> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("MoroccanCIN.Empty", "Le numéro CIN est obligatoire.");
        }


        string normalized = value.Trim().ToUpperInvariant();

        if (!CinRegex.IsMatch(normalized))
        {
            return Error.Validation("MoroccanCIN.Invalid",
                "Le numéro CIN doit être au format marocain valide (ex: BE123456).");
        }


        return new MoroccanCin(normalized);
    }

    public override string ToString() => Value;
}

