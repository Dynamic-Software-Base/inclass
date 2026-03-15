using Domain.EducationalSystem.Entities;
using Domain.Schools.Entities;
using Domain.Schools.Events;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools;

public sealed class School : AggregateRoot<School, SchoolId>
{
    public UserId OwnerUserId { get; private set; }
    public EducationalSystemId EducationalSystemId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Ar_Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;

    public Address Address { get; private set; }
    public SchoolContactInfo ContactInfo { get; private set; }

    private readonly List<SchoolSupportedGrade> _supportedGrades = [];
    public IReadOnlyCollection<SchoolSupportedGrade> SupportedGrades => _supportedGrades;

    /// <summary>
    /// Cached projection of which broad levels this school covers.
    /// Always derived from <see cref="_supportedGrades"/> — never set directly.
    /// Used for fast discovery filtering.
    /// </summary>
    public GradeLevelOffering GradeLevels { get; private set; }



    // pictures
    private readonly List<SchoolPicture> _pictures = [];
    public IReadOnlyCollection<SchoolPicture> Pictures => _pictures.AsReadOnly();
    private School()
    {
    }

    private School(
        SchoolId id,
        UserId createdBy,
        string name,
        string? arabicName,
        UserId ownerUserId,
        EducationalSystemId educationalSystemId,
        Address address,
        SchoolContactInfo contactInfo,
        string? description = null) : base(id, createdBy)
    {
        Name = name;
        OwnerUserId = ownerUserId;
        Address = address;
        ContactInfo = contactInfo;
        Description = description;
        GradeLevels = GradeLevelOffering.Empty;
        Ar_Name = arabicName;
        EducationalSystemId = educationalSystemId;
    }

    public static ErrorOr<School> Create(
        SchoolId id,
        UserId createdBy,
        string name,
        string? arabicName,
        UserId ownerUserId,
        Address address,
        EducationalSystemId educationalSystemId,
        SchoolContactInfo contactInfo,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("school.creation", "name is required");
        }

        School school = new(id, createdBy, name, arabicName, ownerUserId, educationalSystemId, address, contactInfo,
            description);

        school.RaiseDomainEvent(new SchoolCreatedDomainEvent(
            school.Id,
            school.Name,
            school.OwnerUserId));

        return school;
    }

    public ErrorOr<Updated> AddPicture(SchoolPicture pictureId)
    {
        _pictures.Add(pictureId);
        SetUpdated(DateTimeOffset.UtcNow, OwnerUserId);
        return Result.Updated;
    }
    // ── Supported grades ────────────────────────────────────────────────────

    /// <summary>
    /// Declares that this school offers a particular grade.
    /// Automatically recomputes <see cref="GradeLevels"/>.
    /// </summary>
    /// <param name="gradeDefinition">
    /// The GradeDefinition entity — passed in so we can validate it belongs
    /// to this school's EducationSystem and read its cycle's BroadLevel.
    /// </param>
    public ErrorOr<SchoolSupportedGrade> AddSupportedGrade(GradeDefinition gradeDefinition,
        GradeCycleDefinition cycle,
        int? capaciy = null)
    {
        if (cycle.EducationalSystemId != EducationalSystemId)
        {
            return DomainErrors.SchoolErrors.GradeBelongsToDifferentEducationSystem;
        }

        if (!gradeDefinition.IsActive)
        {
            return DomainErrors.SchoolErrors.GradeDefinitionIsInactive;
        }

        if (_supportedGrades.Any(g => g.GradeDefinitionId == gradeDefinition.Id))
        {
            return DomainErrors.SchoolErrors.GradeAlreadySupported;
        }

        var supported = SchoolSupportedGrade.Create(Id, gradeDefinition.Id, cycle.BroadLevel, capaciy);
        _supportedGrades.Add(supported);

        RecomputeGradeLevels();

        return supported;
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private void RecomputeGradeLevels()
    {
        // We store the broad level on SchoolSupportedGrade so we can
        // recompute without loading GradeDefinition + cycle every time.
        GradeLevels = GradeLevelOffering.FromLevels(
            _supportedGrades.Select(g => g.CachedBroadLevel));
    }
}
