using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations.Events;

/// <summary>
/// Raised when a school rejects a student registration.
/// Can be used to notify the applicant via ApplicantContact.
/// </summary>
public sealed record RegistrationRejectedEvent(
    Guid EventId,
    DateTime OccurredOn,
    StudentRegistrationId RegistrationId,
    SchoolId SchoolId,
    string Reason)
    : IDomainEvent;
