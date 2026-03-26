using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationExpiredEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId) : IDomainEvent;
