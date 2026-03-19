using Domain.Schools;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations.Events;

/// <summary>
/// Raised when a school approves a student registration.
/// Handled by Students context — creates Student + ParentTuteur + StudentExtendedData.
/// Carries FormSchemaId so the handler knows which schema to use for field promotion.
/// </summary>
public sealed record RegistrationApprovedEvent(
    Guid EventId,
    DateTime OccurredOn,
    StudentRegistrationId RegistrationId,
    SchoolId SchoolId,
    GradeDefinitionId GradeDefinitionId,
    RegistrationFormSchemaId FormSchemaId,
    AcademicYear AcademicYear)
    : IDomainEvent;
