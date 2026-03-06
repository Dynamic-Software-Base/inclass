using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools.Events;

public sealed record SchoolCreatedDomainEvent(
    SchoolId SchoolId,
    string Name,
    UserId OwnerUserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
