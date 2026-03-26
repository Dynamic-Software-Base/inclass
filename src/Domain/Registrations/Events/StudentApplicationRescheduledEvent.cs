using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationRescheduledEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId,
    DateOnly NewProcessingDate) : IDomainEvent;
