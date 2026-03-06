using SharedKernel.ValueObjects.StronglyTypedIds;

namespace SharedKernel.ValueObjects.Schools;

public sealed record SchoolPicture
{
    public StoredFileId StoredFileId { get; init; }
    public string? AltText { get; init; }
    public bool IsMain { get; init; }
    private SchoolPicture()
    { }
    private SchoolPicture(
        StoredFileId storedFileId,
        string? altText,
        bool isMain)
    {
        StoredFileId = storedFileId;
        AltText = altText;
        IsMain = isMain;
    }

    public static SchoolPicture Create(
        StoredFileId storedFileId,
        string? altText = null,
        bool isMain = false)
    {
        return new SchoolPicture(storedFileId, altText?.Trim(), isMain);
    }

    public SchoolPicture AsMain() => this with { IsMain = true };

    public SchoolPicture AsSecondary() => this with { IsMain = false };

    public SchoolPicture WithAltText(string? altText) => this with { AltText = altText?.Trim() };
}
