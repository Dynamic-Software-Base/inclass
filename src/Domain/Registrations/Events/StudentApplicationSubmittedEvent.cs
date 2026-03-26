using Domain.Registrations.Enums;
using SharedKernel;
using SharedKernel.ValueObjects;

namespace Domain.Registrations.Events;

public sealed record StudentApplicationSubmittedEvent
    (
        Guid EventId,
        DateTime OccurredOn,
        Guid ApplicationId,
        Guid SessionId,
        Guid SchoolId,
        Guid TargetGradeId,
        AcademicYear AcademicYear,
        ApplicantType applicantType): IDomainEvent;
