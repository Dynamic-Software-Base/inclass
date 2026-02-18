using SharedKernel.Enums;

namespace Application.Invitations.Common;

internal static class InvitationTargetMasker
{
    public static string Mask(InvitationTargetType targetType, string normalizedTargetValue) =>
        targetType switch
        {
            InvitationTargetType.Email => MaskEmail(normalizedTargetValue),
            InvitationTargetType.Phone => MaskPhone(normalizedTargetValue),
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, "Unsupported invitation target type.")
        };

    private static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        string trimmedEmail = email.Trim();
        int atIndex = trimmedEmail.IndexOf('@');

        if (atIndex <= 0 || atIndex == trimmedEmail.Length - 1)
        {
            return MaskCore(trimmedEmail);
        }

        string localPart = trimmedEmail[..atIndex];
        string domainPart = trimmedEmail[(atIndex + 1)..];

        return $"{MaskCore(localPart)}@{domainPart}";
    }

    private static string MaskPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return string.Empty;
        }

        string trimmedPhone = phone.Trim();
        int visibleCount = Math.Min(4, trimmedPhone.Length);
        int maskedLength = Math.Max(0, trimmedPhone.Length - visibleCount);
        string visibleSuffix = trimmedPhone[^visibleCount..];

        return string.Concat(new string('*', maskedLength), visibleSuffix);
    }

    private static string MaskCore(string value)
    {
        return value.Length switch
        {
            <= 1 => "*",
            2 => $"{value[0]}*",
            _ => $"{value[0]}{new string('*', value.Length - 2)}{value[^1]}"
        };
    }
}
