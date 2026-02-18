namespace SharedKernel.ValueObjects.StronglyTypedIds;

public  readonly record struct SchoolId(Guid Value)
{
    public static SchoolId New() => new(Guid.NewGuid());
    public static SchoolId From(string id ) => new (Guid.Parse(id));
    public static SchoolId From(Guid id) => new(id);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(SchoolId id) => id.Value;
    public static explicit operator SchoolId(Guid id) => From(id);
}
