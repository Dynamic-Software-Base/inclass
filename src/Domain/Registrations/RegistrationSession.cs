using Domain.Registrations.Enums;
using Domain.Registrations.Events;
using Domain.Registrations.ValueObjects;
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


    public RegistrationCapacity Capacity { get; private set; }


    public RegistrationSessionStatus Status { get; private set; }

    public Guid? BatchId { get; private set; }


    public DailyProcessingQuota? ProcessingQuota { get; private set; }
    public TimeOnly DailyCutoffTime { get; private set; }
    public AssignmentStrategy AssignmentStrategy { get; private set; }
    public int ReservedCount { get; private set; }
    public int EnrolledCount { get; private set; }
    private readonly List<RegistrationPhase> _phases = [];
    public IReadOnlyCollection<RegistrationPhase> Phases => _phases.AsReadOnly();

    private RegistrationSession() { }

    private RegistrationSession(
        RegistrationSessionId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        RegistrationFormSchemaId formSchemaId,
        AcademicYear academicYear,
        RegistrationPeriod period,
        RegistrationCapacity capacity,
        List<RegistrationPhase> phases,
        AssignmentStrategy assignmentStrategy,
        DailyProcessingQuota? processingQuota,
        TimeOnly? dailyCutoffTime,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        FormSchemaId = formSchemaId;
        AcademicYear = academicYear;
        Period = period;
        Capacity = capacity;
        _phases = phases;
        AssignmentStrategy = assignmentStrategy;
        ProcessingQuota = processingQuota;
        DailyCutoffTime = dailyCutoffTime ?? new TimeOnly(12, 0);
        ReservedCount = 0;
        EnrolledCount = 0;

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
        List<RegistrationPhase> phases,
        AssignmentStrategy assignmentStrategy,
        UserId createdBy,
        RegistrationCapacity capacity,
        DailyProcessingQuota? processingQuota = null,
        TimeOnly? dailyCutoffTime = null,
        Guid? batchId = null)
    {
        for (int i = 0; i < phases.Count - 1; i++)
        {
            if (phases[i].EndDate > phases[i + 1].StartDate)
            {
                return Error.Validation(
                    "RegistrationSession.PhasesOverlap",
                    "Les phases de la session ne doivent pas se chevaucher.");
            }
        }
        var session = new RegistrationSession(
            id, schoolId, gradeDefinitionId, formSchemaId,
            academicYear, period, capacity , phases,assignmentStrategy,processingQuota,dailyCutoffTime,createdBy);

        session.BatchId = batchId;
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

    public ErrorOr<Success> UpdateCapacity(RegistrationCapacity capacity, UserId updatedBy)
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
        && (!Capacity.IsFull(currentCount));



    public ErrorOr<Success> ReserveSpot()
    {
        if (Capacity.IsFull(ReservedCount + EnrolledCount))
        {
            return Error.Failure("RegistrationSession.ReserveSpot", "The current registration session is full");
        }

        ReservedCount++;
        return Result.Success;
    }

    public ErrorOr<Success> ReleaseSpot()
    {
        if (ReservedCount <= 0)
        {
            return Error.Failure("RegistrationSession.ReleaseSpot", "no reservation to release");
        }
        ReservedCount--;
        return Result.Success;
    }

    public ErrorOr<Success> EnrollStudent()
    {
        if (ReservedCount <= 0)
        {
            return Error.Failure("RegistrationSession.EnrollStudent",
                "Aucune réservation active à convertir en inscription.");
        }

        ReservedCount--;
        EnrolledCount++;
        return Result.Success;
    }
    public bool IsAcceptingRegistrations(DateTime now) =>
        Status == RegistrationSessionStatus.Open
        && Period.IsOpen(now)
        && !Capacity.IsFull(ReservedCount + EnrolledCount);
    public int GetAvailableSlots() => Capacity.MaxSlots - ReservedCount - EnrolledCount;
    public bool IsWhitelistRequired() => GetAvailableSlots() == 0 && ReservedCount > 0;
    public bool IsHardFull() => GetAvailableSlots() == 0 && ReservedCount == 0;
    public ErrorOr<DateOnly> CalculateProcessingDate(int queuePosition, DateTime submittedAt)
    {
        if (ProcessingQuota != null)
        {
            return ProcessingQuota.CalculateProcessingDate(DateOnly.FromDateTime(Period.OpenDate), queuePosition,
                submittedAt, DailyCutoffTime);
        }

        return Error.Failure("RegistrationSession.CalculateProcessingDate", "No dailyQuta has been set ");
    }
    public RegistrationPhase? GetActivePhase(DateTime now) =>
        _phases.FirstOrDefault(p => p.IsActive(now));
}
