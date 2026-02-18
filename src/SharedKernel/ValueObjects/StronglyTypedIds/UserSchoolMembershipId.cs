namespace SharedKernel.ValueObjects.StronglyTypedIds;

public readonly record struct UserSchoolMembershipId(Guid Value)
{
    public static UserSchoolMembershipId New() => new(Guid.NewGuid());
    public static UserSchoolMembershipId From(Guid id) => new(id);
    public static UserSchoolMembershipId From(string id) => new(Guid.Parse(id));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserSchoolMembershipId id) => id.Value;
    public static explicit operator UserSchoolMembershipId(Guid id) => From(id);
}
