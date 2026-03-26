using Domain.Schools;
using Domain.Students.Enum;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Students;

public sealed class Student : AggregateRoot<Student, StudentId>
{
    public SchoolId SchoolId { get; private set; }
    public StudentRegistrationId RegistrationId { get; private set; }
    public GradeDefinitionId GradeDefinitionId { get; private set; }
    public FullName FullName { get; private set; } = null!;
    public DateOnly DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public NationalId? NationalId { get; private set; }
    public AcademicYear AcademicYear { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Student() { }

    private Student(
        StudentId id,
        SchoolId schoolId,
        StudentRegistrationId registrationId,
        GradeDefinitionId gradeDefinitionId,
        FullName fullName,
        DateOnly dateOfBirth,
        Gender gender,
        AcademicYear academicYear,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        RegistrationId = registrationId;
        GradeDefinitionId = gradeDefinitionId;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        AcademicYear = academicYear;
    }

    public static ErrorOr<Student> Create(
        StudentId id,
        SchoolId schoolId,
        StudentRegistrationId registrationId,
        GradeDefinitionId gradeDefinitionId,
        FullName fullName,
        DateOnly dateOfBirth,
        Gender gender,
        AcademicYear academicYear,
        UserId createdBy)
    {
        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Error.Validation("Student.DateOfBirth.Invalid",
                "La date de naissance doit être dans le passé.");
        }


        return new Student(id, schoolId, registrationId, gradeDefinitionId,
            fullName, dateOfBirth, gender, academicYear, createdBy);
    }

    // --- Mutation ---

    public ErrorOr<Success> AssignNationalId(NationalId nationalId, UserId updatedBy)
    {
        if (NationalId is not null)
        {
            return Error.Conflict("Student.NationalId.AlreadyAssigned",
                "Un identifiant national est déjà assigné à cet élève.");

        }

        NationalId = nationalId;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    public void Deactivate(UserId updatedBy)
    {
        IsActive = false;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
    }

    public void Activate(UserId updatedBy)
    {
        IsActive = true;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
    }
}
