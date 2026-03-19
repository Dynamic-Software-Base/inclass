using Domain.Schools;
using Domain.Students.Enum;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Registration;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Students;

/// <summary>
/// Élève inscrit dans une école.
/// Créé uniquement via RegistrationApprovedEvent — jamais directement.
/// Les champs proviennent de la section "personal" du FormValuesJson.
/// </summary>
public sealed class Student : AggregateRoot<Student, StudentId>
{
    /// <summary>École dans laquelle l'élève est inscrit</summary>
    public SchoolId SchoolId { get; private set; }

    /// <summary>Dossier d'inscription source</summary>
    public StudentRegistrationId RegistrationId { get; private set; }

    /// <summary>Niveau scolaire actuel</summary>
    public GradeDefinitionId GradeDefinitionId { get; private set; }

    /// <summary>Nom complet — prénom + nom de famille</summary>
    public FullName FullName { get; private set; } = null!;

    /// <summary>Date de naissance</summary>
    public DateOnly DateOfBirth { get; private set; }

    /// <summary>Sexe</summary>
    public Gender Gender { get; private set; }

    /// <summary>
    /// Identifiant national — code MASSAR ou CIN.
    /// Nullable : peut ne pas être assigné au moment de l'inscription.
    /// </summary>
    public NationalId? NationalId { get; private set; }

    /// <summary>Année scolaire d'inscription — ex: "2024-2025"</summary>
    public AcademicYear AcademicYear { get; private set; } = null!;

    /// <summary>L'élève est-il actif dans l'école ?</summary>
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
