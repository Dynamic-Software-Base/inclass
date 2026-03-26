using SharedKernel;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationWaitlistedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ApplicationId,
    Guid SessionId,
    Guid SchoolId,
    int WaitlistPosition) : IDomainEvent;
