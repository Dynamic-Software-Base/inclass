using Domain.Schools.Events;
using SharedKernel;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools;

public sealed class School : AggregateRoot<School, SchoolId>
{
    public UserId OwnerUserId { get; private set; }

    private readonly List<SchoolPicture> _pictures = [];
    public string Name { get; private set; } = string.Empty;
    public string? Ar_Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;

    public Address Address { get; private set; }
    public SchoolContactInfo ContactInfo { get; private set; }
    public GradeLevelOffering GradeLevels { get; private set; }

    // pictures
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
        Address address,
        SchoolContactInfo contactInfo,
        GradeLevelOffering gradeLevels,
        string? description = null) : base(id, createdBy)
    {
        Name = name;
        OwnerUserId = ownerUserId;
        Address = address;
        ContactInfo = contactInfo;
        Description = description;
        GradeLevels = gradeLevels;
        Ar_Name = arabicName;
    }

    public static ErrorOr<School> Create(
        SchoolId id,
        UserId createdBy,
        string name,
        string? arabicName,
        UserId ownerUserId,
        Address address,
        SchoolContactInfo contactInfo,
        GradeLevelOffering gradeLevels,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("school.creation", "name is required");
        }
        School school = new(id, createdBy, name,arabicName, ownerUserId,address, contactInfo,gradeLevels, description);

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
}
