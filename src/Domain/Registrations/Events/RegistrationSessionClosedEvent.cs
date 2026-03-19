using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations.Events;

/// <summary>
/// Raised when a registration session is closed (manually or by school).
/// </summary>
public sealed record RegistrationSessionClosedEvent(
    Guid EventId,
    DateTime OccurredOn,
    RegistrationSessionId SessionId,
    SchoolId SchoolId)
    : IDomainEvent;
