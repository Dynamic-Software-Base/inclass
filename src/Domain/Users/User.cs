using Domain.Common;
using Domain.Users.Events;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Users;

public sealed class User : AggregateRoot<User, UserId>
{
    public string KeycloakUserId { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private User()
    {
    }

    private User(
        UserId id,
        UserId createdBy,
        string keycloakUserId,
        string fullName,
        string? email,
        string? phoneNumber,
        bool isActive) : base(id, createdBy)
    {
        KeycloakUserId = NormalizeRequired(keycloakUserId);
        FullName = NormalizeRequired(fullName);
        Email = ContactTargetNormalizer.NormalizeOptionalEmail(email);
        PhoneNumber = ContactTargetNormalizer.NormalizeOptionalPhoneNumber(phoneNumber);
        IsActive = isActive;
    }

    public static User Create(
        UserId id,
        UserId createdBy,
        string keycloakUserId,
        string fullName,
        string? email = null,
        string? phoneNumber = null,
        bool isActive = true)
    {
        User user = new(id, createdBy, keycloakUserId, fullName, email, phoneNumber, isActive);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id,
            user.KeycloakUserId,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.IsActive));

        return user;
    }

    public string? GetNormalizedTargetValue(InvitationTargetType targetType) =>
        targetType switch
        {
            InvitationTargetType.Email => Email,
            InvitationTargetType.Phone => PhoneNumber,
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, "Unsupported invitation target type.")
        };

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
