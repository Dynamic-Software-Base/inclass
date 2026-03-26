using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationApprovedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId,
    Guid AssignedClassId) : IDomainEvent;
