using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Users.Events;

public sealed record UserCreatedDomainEvent(
    UserId UserId,
    string KeycloakUserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    bool IsActive) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
