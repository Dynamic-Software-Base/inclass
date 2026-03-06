using System.Security.Cryptography;
using System.Text;

namespace Application.Invitations.Common;

internal static class InvitationTokenHasher
{
    public static string GenerateRawToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToHexString(bytes);
    }

    public static string ComputeHash(string rawToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);

        string normalizedToken = rawToken.Trim().ToUpperInvariant();
        byte[] tokenBytes = Encoding.UTF8.GetBytes(normalizedToken);
        byte[] hashBytes = SHA512.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
