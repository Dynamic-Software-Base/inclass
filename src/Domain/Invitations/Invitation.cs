using Domain.Common;
using Domain.Users;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Invitations;

public sealed class Invitation : AggregateRoot<Invitation, InvitationId>
{
    public SchoolId SchoolId { get; private set; }
    public UserRole Role { get; private set; }
    public InvitationTargetType TargetType { get; private set; }
    public string TargetValue { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public UserId? AcceptedByUserId { get; private set; }

    private Invitation()
    {
    }

    private Invitation(
        InvitationId id,
        UserId createdBy,
        SchoolId schoolId,
        UserRole role,
        InvitationTargetType targetType,
        string targetValue,
        string tokenHash,
        DateTimeOffset expiresAt) : base(id, createdBy)
    {
        SchoolId = schoolId;
        Role = role;
        TargetType = targetType;
        TargetValue = ContactTargetNormalizer.Normalize(targetType, targetValue);
        TokenHash = NormalizeRequired(tokenHash);
        ExpiresAt = expiresAt;
        Status = InvitationStatus.Pending;
    }

    public static Invitation Create(
        InvitationId id,
        UserId createdBy,
        SchoolId schoolId,
        UserRole role,
        InvitationTargetType targetType,
        string targetValue,
        string tokenHash,
        DateTimeOffset expiresAt) =>
        new(id, createdBy, schoolId, role, targetType, targetValue, tokenHash, expiresAt);

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    public void Accept(User user, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(user);

        EnsureCanAccept(now);

        string? userTargetValue = user.GetNormalizedTargetValue(TargetType);
        if (string.IsNullOrWhiteSpace(userTargetValue) ||
            !string.Equals(TargetValue, userTargetValue, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invitation target does not match the user.");
        }

        Status = InvitationStatus.Accepted;
        AcceptedAt = now;
        AcceptedByUserId = user.Id;
        SetUpdated(now, user.Id);
    }

    public void Revoke(DateTimeOffset now)
    {
        if (Status == InvitationStatus.Accepted)
        {
            throw new InvalidOperationException("Accepted invitations cannot be revoked.");
        }

        if (Status == InvitationStatus.Revoked || Status == InvitationStatus.Expired)
        {
            return;
        }

        if (IsExpired(now))
        {
            Status = InvitationStatus.Expired;
            SetUpdated(now, CreatedBy);
            return;
        }

        Status = InvitationStatus.Revoked;
        SetUpdated(now, CreatedBy);
    }

    private void EnsureCanAccept(DateTimeOffset now)
    {
        if (Status == InvitationStatus.Accepted)
        {
            throw new InvalidOperationException("Invitation has already been accepted.");
        }

        if (Status == InvitationStatus.Revoked)
        {
            throw new InvalidOperationException("Invitation has been revoked.");
        }

        if (Status == InvitationStatus.Expired || IsExpired(now))
        {
            Status = InvitationStatus.Expired;
            throw new InvalidOperationException("Invitation has expired.");
        }
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
