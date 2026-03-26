using SharedKernel;
using SharedKernel.ValueObjects;

namespace Domain.Schools.Events;

public sealed record SchoolClassCreatedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ClassId,
    Guid SchoolId,
    Guid GradeDefinitionId,
    AcademicYear AcademicYear) : IDomainEvent;
