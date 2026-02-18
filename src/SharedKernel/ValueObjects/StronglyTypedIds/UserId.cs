namespace SharedKernel.ValueObjects.StronglyTypedIds;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid id) => new(id);
    public static UserId From(string id) => new(Guid.Parse(id));

    public override string ToString() => Value.ToString();


    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid id) => From(id);
}
