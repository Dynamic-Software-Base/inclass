using Domain.Schools.Enums;
using Domain.Schools.Events;
using Domain.Schools.ValueObjects;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Domain.Schools;

public sealed class SchoolClass : AggregateRoot<SchoolClass, SchoolClassId>
{
    public SchoolId SchoolId { get; private set; }
    public GradeDefinitionId GradeDefinitionId { get; private set; }
    public AcademicYear AcademicYear { get; private set; } = null!;
    public ClassName Name { get; private set; } = null!;
    public ClassCapacity Capacity { get; private set; } = null!;
    public int CurrentEnrollmentCount { get; private set; }

    private SchoolClass() { }

    private SchoolClass(
        SchoolClassId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        AcademicYear academicYear,
        ClassName name,
        ClassCapacity capacity,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        GradeDefinitionId = gradeDefinitionId;
        AcademicYear = academicYear;
        Name = name;
        Capacity = capacity;
        CurrentEnrollmentCount = 0;
    }

    public static ErrorOr<SchoolClass> Create(
        SchoolClassId id,
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        AcademicYear academicYear,
        ClassName name,
        ClassCapacity capacity,
        UserId createdBy)
    {
        var schoolClass = new SchoolClass(
            id, schoolId, gradeDefinitionId,
            academicYear, name, capacity, createdBy);

        schoolClass.RaiseDomainEvent(new SchoolClassCreatedEvent(
            Guid.NewGuid(), DateTime.UtcNow,
            id, schoolId, gradeDefinitionId, academicYear));

        return schoolClass;
    }

    public ErrorOr<Success> EnrollStudent(UserId updatedBy)
    {
        if (Capacity.IsFull(CurrentEnrollmentCount))
        {
            return Error.Conflict("SchoolClass.Full",
                "La classe est complète, impossible d'inscrire un élève supplémentaire.");
        }

        CurrentEnrollmentCount++;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    public ErrorOr<Success> UnenrollStudent(UserId updatedBy)
    {
        if (CurrentEnrollmentCount <= 0)
        {
            return Error.Conflict("SchoolClass.Empty",
                "Aucun élève inscrit dans cette classe.");
        }

        CurrentEnrollmentCount--;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateName(ClassName name, UserId updatedBy)
    {
        Name = name;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateCapacity(ClassCapacity capacity, UserId updatedBy)
    {
        if (capacity.MaxStudents < CurrentEnrollmentCount)
        {
            return Error.Conflict("SchoolClass.CapacityBelowEnrollment",
                "La nouvelle capacité ne peut pas être inférieure au nombre d'élèves actuellement inscrits.");
        }

        Capacity = capacity;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);
        return Result.Success;
    }
}
