namespace SharedKernel.ValueObjects.StronglyTypedIds;

public readonly record struct StoredFileId(Guid Value)
{
    public static  StoredFileId Empty => new(Guid.Empty);
    public static StoredFileId From(Guid value) => new StoredFileId(value);
    public static StoredFileId From(string value) => new StoredFileId(Guid.Parse(value));
    public static StoredFileId New() => new StoredFileId(Guid.NewGuid());
    public override string ToString() => Value.ToString();




    public static implicit operator Guid(StoredFileId id) => id.Value;
    public static explicit operator StoredFileId(Guid id) => new StoredFileId(id);
}
