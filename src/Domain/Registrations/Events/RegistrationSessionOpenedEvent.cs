using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations.Events;


/// <summary>
/// Raised when a registration session transitions to Open status.
/// </summary>
public sealed record RegistrationSessionOpenedEvent(
    Guid EventId,
    DateTime OccurredOn,
    RegistrationSessionId SessionId,
    SchoolId SchoolId,
    GradeDefinitionId GradeDefinitionId,
    AcademicYear AcademicYear)
    : IDomainEvent;
