using Domain.Registrations.Enums;
using Domain.Registrations.Events;
using Domain.Schools;
using ErrorOr;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Registrations;

public sealed class RegistrationSession
    : AggregateRoot<RegistrationSession, RegistrationSessionId>
{
    public SchoolId SchoolId { get; private set; }


    public GradeDefinitionId GradeDefinitionId { get; private set; }


    public RegistrationFormSchemaId FormSchemaId { get; private set; }


    public AcademicYear AcademicYear { get; private set; } = null!;


    public RegistrationPeriod Period { get; private set; } = null!;


    public RegistrationCapacity? Capacity { get; private set; }


    public RegistrationSessionStatus Status { get; private set; }

    public Guid? BatchId { get; private set; }
    private RegistrationSession() { }

    private RegistrationSession(
        RegistrationSessionId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        RegistrationFormSchemaId formSchemaId,
        AcademicYear academicYear,
        RegistrationPeriod period,
        RegistrationCapacity? capacity,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        FormSchemaId = formSchemaId;
        AcademicYear = academicYear;
        Period = period;
        Capacity = capacity;

        // Determine initial status from period
        Status = period.IsScheduled(DateTime.UtcNow)
            ? RegistrationSessionStatus.Scheduled
            : RegistrationSessionStatus.Open;
    }

    public static ErrorOr<RegistrationSession> Create(
        RegistrationSessionId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        RegistrationFormSchemaId formSchemaId,
        AcademicYear academicYear,
        RegistrationPeriod period,
        UserId createdBy,
        RegistrationCapacity? capacity = null,
        Guid? batchId = null)
    {
        var session = new RegistrationSession(
            id, schoolId, gradeDefinitionId, formSchemaId,
            academicYear, period, capacity, createdBy);
        session.BatchId = batchId;
        // Only raise opened event if session starts immediately
        if (session.Status == RegistrationSessionStatus.Open)
        {
            session.RaiseDomainEvent(new RegistrationSessionOpenedEvent(
                Guid.NewGuid(), DateTime.UtcNow,
                id, schoolId, gradeDefinitionId, academicYear));
        }

        return session;
    }

    // --- Lifecycle ---

    /// <summary>
    /// Transitions a Scheduled session to Open.
    /// Called when OpenDate is reached (checked by caller, e.g. background job or on-demand).
    /// </summary>
    public ErrorOr<Success> Open(UserId updatedBy)
    {
        if (Status != RegistrationSessionStatus.Scheduled)
        {
            return Error.Conflict("Session.NotScheduled",
                "Seules les sessions planifiées peuvent être ouvertes.");
        }


        Status = RegistrationSessionStatus.Open;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);

        RaiseDomainEvent(new RegistrationSessionOpenedEvent(
            Guid.NewGuid(), DateTime.UtcNow,
            Id, SchoolId, GradeDefinitionId, AcademicYear));

        return Result.Success;
    }

    public ErrorOr<Success> Close(UserId closedBy)
    {
        if (Status is not (RegistrationSessionStatus.Open or RegistrationSessionStatus.Scheduled))
        {
            return Error.Conflict("Session.AlreadyClosed", "La session est déjà fermée ou annulée.");
        }


        Status = RegistrationSessionStatus.Closed;
        SetUpdated(DateTimeOffset.UtcNow, closedBy);

        RaiseDomainEvent(new RegistrationSessionClosedEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, SchoolId));

        return Result.Success;
    }

    public ErrorOr<Success> Cancel(UserId cancelledBy)
    {
        if (Status is not RegistrationSessionStatus.Scheduled)
        {
            return Error.Conflict("Session.CannotCancel",
                "Seules les sessions planifiées peuvent être annulées.");
        }


        Status = RegistrationSessionStatus.Cancelled;
        SetUpdated(DateTimeOffset.UtcNow, cancelledBy);

        RaiseDomainEvent(new RegistrationSessionCancelledEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, SchoolId));

        return Result.Success;
    }

    public ErrorOr<Success> UpdateCapacity(RegistrationCapacity? capacity, UserId updatedBy)
    {
        if (Status == RegistrationSessionStatus.Closed)
        {
            return Error.Conflict("Session.Closed", "Impossible de modifier une session fermée.");
        }


        Capacity = capacity;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    public ErrorOr<Success> UpdatePeriod(RegistrationPeriod period, UserId updatedBy)
    {
        if (Status == RegistrationSessionStatus.Closed)
        {
            return Error.Conflict("Session.Closed", "Impossible de modifier une session fermée.");
        }


        Period = period;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    // --- Query helpers ---
    public bool IsAcceptingRegistrations(DateTime now, int currentCount) =>
        Status == RegistrationSessionStatus.Open
        && Period.IsOpen(now)
        && (Capacity is null || !Capacity.IsFull(currentCount));
}
