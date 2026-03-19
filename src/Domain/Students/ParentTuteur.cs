using Domain.Schools;
using Domain.Students.Enum;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.Registration;

namespace Domain.Students;

/// <summary>
/// Parent ou tuteur légal d'un élève.
/// Créé via RegistrationApprovedEvent — champs depuis la section "parent" du FormValuesJson.
/// Un même ParentTuteur peut être lié à plusieurs élèves (fratrie).
/// </summary>
public sealed class ParentTuteur : AggregateRoot<ParentTuteur, ParentTuteurId>
{
    /// <summary>École concernée</summary>
    public SchoolId SchoolId { get; private set; }

    /// <summary>Dossier d'inscription source</summary>
    public StudentRegistrationId RegistrationId { get; private set; }

    /// <summary>Nom complet du parent / tuteur</summary>
    public FullName FullName { get; private set; } = null!;

    /// <summary>CIN marocaine du tuteur</summary>
    public MoroccanCin CIN { get; private set; } = null!;

    /// <summary>Téléphone principal</summary>
    public PhoneNumber PhoneNumber { get; private set; } = null!;

    /// <summary>Téléphone secondaire — optionnel</summary>
    public PhoneNumber? SecondaryPhoneNumber { get; private set; }

    /// <summary>Adresse du domicile — optionnelle</summary>
    public Address? Address { get; private set; }

    /// <summary>Lien de parenté avec l'élève</summary>
    public ParentalRelation Relation { get; private set; }

    /// <summary>Ce tuteur est-il le tuteur légal officiel ?</summary>
    public bool IsLegalGuardian { get; private set; }

    /// <summary>Email — pour notifications école</summary>
    public Email? Email { get; private set; }

    private readonly List<StudentId> _studentIds = [];

    /// <summary>
    /// Élèves liés à ce tuteur.
    /// Permet à un parent de couvrir plusieurs enfants dans la même école.
    /// </summary>
    public IReadOnlyList<StudentId> StudentIds => _studentIds.AsReadOnly();

    private ParentTuteur() { }

    private ParentTuteur(
        ParentTuteurId id,
        SchoolId schoolId,
        StudentRegistrationId registrationId,
        FullName fullName,
        MoroccanCin cin,
        PhoneNumber phoneNumber,
        Address? address,
        ParentalRelation relation,
        bool isLegalGuardian,
        Email? email,
        UserId createdBy)
        : base(id, createdBy)
    {
        SchoolId = schoolId;
        RegistrationId = registrationId;
        FullName = fullName;
        CIN = cin;
        PhoneNumber = phoneNumber;
        Address = address;
        Relation = relation;
        IsLegalGuardian = isLegalGuardian;
        Email = email;
    }

    public static ErrorOr<ParentTuteur> Create(
        ParentTuteurId id,
        SchoolId schoolId,
        StudentRegistrationId registrationId,
        FullName fullName,
        MoroccanCin cin,
        PhoneNumber phoneNumber,
        ParentalRelation relation,
        bool isLegalGuardian,
        UserId createdBy,
        Address? address = null,
        Email? email = null)
    {
        return new ParentTuteur(id, schoolId, registrationId, fullName, cin,
            phoneNumber, address, relation, isLegalGuardian, email, createdBy);
    }

    // --- Mutation ---

    public ErrorOr<Success> LinkStudent(StudentId studentId)
    {
        if (_studentIds.Contains(studentId))
        {
            return Error.Conflict("ParentTuteur.Student.AlreadyLinked",
                "Cet élève est déjà lié à ce tuteur.");

        }

        _studentIds.Add(studentId);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateContact(
        PhoneNumber phoneNumber,
        PhoneNumber? secondaryPhone,
        Email? email,
        Address? address,
        UserId updatedBy)
    {
        if (secondaryPhone is not null && secondaryPhone.Number == phoneNumber.Number)
        {
            return Error.Validation("ParentTuteur.SecondaryPhone.SameAsPrimary",
                "Le numéro secondaire doit être différent du numéro principal.");
        }


        PhoneNumber = phoneNumber;
        SecondaryPhoneNumber = secondaryPhone;
        Email = email;
        Address = address;
        SetUpdated(DateTimeOffset.UtcNow, updatedBy);

        return Result.Success;
    }
}
