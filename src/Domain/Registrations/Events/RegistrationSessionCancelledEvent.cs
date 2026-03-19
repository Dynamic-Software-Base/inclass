using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations.Events;

/// <summary>
/// Raised when a registration session is cancelled before it opened.
/// </summary>
public sealed record RegistrationSessionCancelledEvent(
    Guid EventId,
    DateTime OccurredOn,
    RegistrationSessionId SessionId,
    SchoolId SchoolId)
    : IDomainEvent;
