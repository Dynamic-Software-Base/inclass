using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationReservedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId,
    DateOnly ProcessingDate) : IDomainEvent;
