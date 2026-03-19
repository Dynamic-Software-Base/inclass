using System.Text.RegularExpressions;

namespace SharedKernel.ValueObjects.Registration;

/// <summary>
/// Identifiant national de l'élève — code MASSAR ou CIN.
/// </summary>
public sealed record NationalId
{
    private static readonly Regex MassarRegex = new(
        @"^[A-Za-z]\d{9}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private static readonly Regex CinRegex = new(
        @"^[A-Za-z]{1,2}\d{5,6}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public string Value { get; }
    public NationalIdType Type { get; }

    private NationalId(string value, NationalIdType type)
    {
        Value = value;
        Type = type;
    }

    public static ErrorOr<NationalId> Create(string value, NationalIdType type)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("NationalId.Empty", "L'identifiant national est obligatoire.");
        }


        string normalized = value.Trim().ToUpperInvariant();

        return type switch
        {
            NationalIdType.Massar when !MassarRegex.IsMatch(normalized) =>
                Error.Validation("NationalId.Massar.Invalid",
                    "Le code MASSAR doit contenir une lettre suivie de 9 chiffres (ex: G123456789)."),

            NationalIdType.CIN when !CinRegex.IsMatch(normalized) =>
                Error.Validation("NationalId.CIN.Invalid",
                    "Le numéro CIN doit être au format marocain valide (ex: BE123456)."),

            _ => new NationalId(normalized, type)
        };
    }

    public override string ToString() => $"{Type}: {Value}";
}

public enum NationalIdType
{
    /// <summary>Identifiant du système MASSAR du MEN</summary>
    Massar = 1,

    /// <summary>Carte Nationale d'Identité</summary>
    CIN = 2
}
