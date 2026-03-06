using System.Text;
using SharedKernel.Enums;

namespace Domain.Common;

internal static class ContactTargetNormalizer
{
    public static string Normalize(InvitationTargetType targetType, string targetValue) =>
        targetType switch
        {
            InvitationTargetType.Email => NormalizeEmail(targetValue),
            InvitationTargetType.Phone => NormalizePhoneNumber(targetValue),
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, "Unsupported invitation target type.")
        };

    public static string? NormalizeOptionalEmail(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : NormalizeEmail(email);

    public static string? NormalizeOptionalPhoneNumber(string? phoneNumber) =>
        string.IsNullOrWhiteSpace(phoneNumber) ? null : NormalizePhoneNumber(phoneNumber);

    private static string NormalizeEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return email.Trim().ToUpperInvariant();
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        string value = phoneNumber.Trim();
        StringBuilder normalized = new(value.Length);

        foreach (char character in value)
        {
            bool isLeadingPlus = character == '+' && normalized.Length == 0;
            if (char.IsDigit(character) || isLeadingPlus)
            {
                normalized.Append(character);
            }
        }

        if (normalized.Length == 0 || (normalized.Length == 1 && normalized[0] == '+'))
        {
            throw new ArgumentException("Phone number must include at least one digit.", nameof(phoneNumber));
        }

        return normalized.ToString();
    }
}
