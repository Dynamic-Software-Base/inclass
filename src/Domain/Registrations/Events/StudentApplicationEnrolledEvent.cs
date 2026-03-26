using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationEnrolledEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId,
    Guid StudentId) : IDomainEvent;
