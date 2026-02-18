using Domain.Schools.Events;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.Schools;

public sealed class School : AggregateRoot<School, SchoolId>
{
    public string Name { get; private set; } = string.Empty;
    public UserId OwnerUserId { get; private set; }

    private School()
    {
    }

    private School(
        SchoolId id,
        UserId createdBy,
        string name,
        UserId ownerUserId) : base(id, createdBy)
    {
        Name = NormalizeRequired(name);
        OwnerUserId = ownerUserId;
    }

    public static School Create(
        SchoolId id,
        UserId createdBy,
        string name,
        UserId ownerUserId)
    {
        School school = new(id, createdBy, name, ownerUserId);

        school.RaiseDomainEvent(new SchoolCreatedDomainEvent(
            school.Id,
            school.Name,
            school.OwnerUserId));

        return school;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
