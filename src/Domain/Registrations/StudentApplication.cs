using Domain.Registrations.Enums;
using Domain.Registrations.Events;
using Domain.Registrations.ValueObjects;
using Domain.Schools;
using Domain.Students;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Domain.Registrations;

public sealed class StudentApplication : AggregateRoot<StudentApplication,StudentApplicationId>
{
    public RegistrationSessionId SessionId { get; private set; }
    public SchoolId SchoolId { get; private set; }
    public GradeDefinitionId TargetGradeId { get; private set; }
    public AcademicYear AcademicYear { get; private set; } = null!;
    public ApplicantType  ApplicationType { get; private set; }
    public StudentApplicationStatus Status { get; private set; }


    public DateTime SubmittedAt { get; private set; }
    public QueueInfo? QueueInfo { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool RescheduleUsed { get; private set; }
    public SchoolClassId? AssignedClassId { get; private set; }
    public PaymentStatus? PaymentStatus { get; private set; }
    public StudentId? StudentId { get; private set; }
    public ParentTuteurId ParentId { get; private set; }
    public string StudentFirstName { get; private set; } = string.Empty;
    public string StudentLastName { get; private set; } = string.Empty;

    public ApplicantContact Contact { get; private set; } = null!;
    public string IdentityKey { get; private set; } = string.Empty;
    public string FormValuesJson { get; private set; } = "{}";
    private StudentApplication() { }

    private StudentApplication(
        StudentApplicationId id,
        RegistrationSessionId sessionId,
        SchoolId schoolId,
        GradeDefinitionId targetGradeId,
        AcademicYear academicYear,
        ApplicantType applicationType,
        StudentId? studentId,
        ParentTuteurId parentId,
        ApplicantContact contact,
        string formValuesJson,
        string identityKey,
        string studentFirstName,
        string studentLastName,
        UserId createdBy)
        : base(id, createdBy)
    {
        StudentFirstName = studentFirstName;
        StudentLastName = studentLastName;
        SessionId = sessionId;
        SchoolId = schoolId;
        TargetGradeId = targetGradeId;
        AcademicYear = academicYear;
        ApplicationType = applicationType;
        StudentId = studentId;
        ParentId = parentId;
        Contact = contact;
        FormValuesJson = formValuesJson;
        IdentityKey = identityKey;
        Status = StudentApplicationStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        RescheduleUsed = false;
    }
    public static ErrorOr<StudentApplication> Create(
        StudentApplicationId id,
        RegistrationSessionId sessionId,
        SchoolId schoolId,
        GradeDefinitionId targetGradeId,
        AcademicYear academicYear,
        ApplicantType applicationType,
        StudentId? studentId,
        ParentTuteurId parentId,
        ApplicantContact contact,
        string formValuesJson,
        string studentFirstName,
        string studentLastName,
        UserId createdBy)
    {
        if (!string.IsNullOrWhiteSpace(studentFirstName))
        {
            return DomainErrors.Required(nameof(studentFirstName));
        }
        if (!string.IsNullOrWhiteSpace(studentLastName))
        {
            return DomainErrors.Required(nameof(studentLastName));
        }
        string identityKey = SharedKernel.ValueObjects.IdentityKey.Generate().Value;

        var application = new StudentApplication(
            id, sessionId, schoolId, targetGradeId,
            academicYear, applicationType, studentId, parentId,
            contact, formValuesJson, identityKey,studentFirstName, studentLastName,createdBy);

        application.RaiseDomainEvent(new StudentApplicationSubmittedEvent(
            Guid.NewGuid(), DateTime.UtcNow,
            id, sessionId, schoolId, targetGradeId, academicYear, applicationType));

        return application;
    }

    public void AssignStudent(StudentId studentId)
    {
        StudentId = studentId;
    }
public ErrorOr<Success> Reserve(QueueInfo queueInfo, DateTime expiryDate)
{
    if (Status != StudentApplicationStatus.Submitted)
    {
        return Error.Conflict("StudentApplication.Reserve",
            "Seule une candidature soumise peut être réservée.");
    }

    QueueInfo = queueInfo;
    ExpiryDate = expiryDate;
    Status = StudentApplicationStatus.Reserved;

    RaiseDomainEvent(new StudentApplicationReservedEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        queueInfo.ProcessingDate));

    return Result.Success;
}

public ErrorOr<Success> AddToWaitlist(int waitlistPosition)
{
    if (Status != StudentApplicationStatus.Submitted)
    {
        return Error.Conflict("StudentApplication.Waitlist",
            "Seule une candidature soumise peut être mise en liste d'attente.");
    }

    Status = StudentApplicationStatus.Waitlisted;

    RaiseDomainEvent(new StudentApplicationWaitlistedEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        waitlistPosition));

    return Result.Success;
}

public ErrorOr<Success> PromoteFromWaitlist(QueueInfo queueInfo, DateTime expiryDate)
{
    if (Status != StudentApplicationStatus.Waitlisted)
    {
        return Error.Conflict("StudentApplication.PromoteFromWaitlist",
            "Seule une candidature en liste d'attente peut être promue.");
    }

    QueueInfo = queueInfo;
    ExpiryDate = expiryDate;
    Status = StudentApplicationStatus.Reserved;

    RaiseDomainEvent(new WaitlistPromotedEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        queueInfo.ProcessingDate));

    return Result.Success;
}

public ErrorOr<Success> MarkUnderReview()
{
    if (Status != StudentApplicationStatus.Reserved)
    {
        return Error.Conflict("StudentApplication.MarkUnderReview",
            "Seule une candidature réservée peut être mise en cours d'examen.");
    }

    Status = StudentApplicationStatus.UnderReview;

    return Result.Success;
}

public ErrorOr<Success> Approve(SchoolClassId assignedClassId, UserId approvedBy)
{
    if (Status != StudentApplicationStatus.UnderReview)
    {
        return Error.Conflict("StudentApplication.Approve",
            "Seule une candidature en cours d'examen peut être approuvée.");
    }

    AssignedClassId = assignedClassId;
    PaymentStatus = Enums.PaymentStatus.PendingPayment;
    Status = StudentApplicationStatus.Approved;
    SetUpdated(DateTimeOffset.UtcNow, approvedBy);

    RaiseDomainEvent(new StudentApplicationApprovedEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        assignedClassId.Value));

    return Result.Success;
}

public ErrorOr<Success> Enroll(UserId enrolledBy)
{
    if (Status != StudentApplicationStatus.Approved)
    {
        return Error.Conflict("StudentApplication.Enroll",
            "Seule une candidature approuvée peut être inscrite.");
    }

    Status = StudentApplicationStatus.Enrolled;
    SetUpdated(DateTimeOffset.UtcNow, enrolledBy);

    RaiseDomainEvent(new StudentApplicationEnrolledEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        StudentId!.Value));

    return Result.Success;
}

public ErrorOr<Success> Reject(UserId rejectedBy)
{
    if (Status is not (StudentApplicationStatus.UnderReview or StudentApplicationStatus.Reserved))
    {
        return Error.Conflict("StudentApplication.Reject",
            "Seule une candidature en cours d'examen ou réservée peut être rejetée.");
    }

    Status = StudentApplicationStatus.Rejected;
    SetUpdated(DateTimeOffset.UtcNow, rejectedBy);

    RaiseDomainEvent(new StudentApplicationRejectedEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value));

    return Result.Success;
}

public ErrorOr<Success> Expire()
{
    if (Status != StudentApplicationStatus.Reserved)
    {
        return Error.Conflict("StudentApplication.Expire",
            "Seule une candidature réservée peut expirer.");
    }

    if (!RescheduleUsed)
    {
        return Error.Conflict("StudentApplication.Expire",
            "Une candidature ne peut expirer qu'après avoir utilisé son report.");
    }

    Status = StudentApplicationStatus.Expired;

    RaiseDomainEvent(new StudentApplicationExpiredEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value));

    return Result.Success;
}

public ErrorOr<Success> Reschedule(DateOnly newProcessingDate, DateTime newExpiryDate)
{
    if (Status != StudentApplicationStatus.Reserved)
    {
        return Error.Conflict("StudentApplication.Reschedule",
            "Seule une candidature réservée peut être reportée.");
    }

    if (RescheduleUsed)
    {
        return Error.Conflict("StudentApplication.Reschedule",
            "Un seul report est autorisé par candidature.");
    }

    RescheduleUsed = true;
    ExpiryDate = newExpiryDate;
    Status = StudentApplicationStatus.Rescheduled;

    QueueInfo? updatedQueue = QueueInfo is not null
        ? new QueueInfo(QueueInfo.Position, newProcessingDate)
        : null;
    QueueInfo = updatedQueue;

    RaiseDomainEvent(new StudentApplicationRescheduledEvent(
        Guid.NewGuid(), DateTime.UtcNow,
        Id.Value, SessionId.Value, SchoolId.Value,
        newProcessingDate));

    return Result.Success;
}
}
