using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationRejectedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId) : IDomainEvent;
