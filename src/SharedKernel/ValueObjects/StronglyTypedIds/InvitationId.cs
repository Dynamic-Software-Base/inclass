namespace SharedKernel.ValueObjects.StronglyTypedIds;

public readonly record struct InvitationId(Guid Value)
{
    public static InvitationId New() => new(Guid.NewGuid());
    public static InvitationId From(Guid id) => new(id);
    public static InvitationId From(string id) => new(Guid.Parse(id));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(InvitationId id) => id.Value;
    public static explicit operator InvitationId(Guid id) => From(id);
}
