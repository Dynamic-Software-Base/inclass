using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Domain.File;

public class StoredFile : AggregateRoot<StoredFile,StoredFileId>
{
    public UserId OwnerId { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoredFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string StorageProvider { get; private set; } = string.Empty;
    public string? BlobPath { get; private set; }

    private StoredFile() : base() { }

    private StoredFile(
        StoredFileId id,
        UserId ownerId,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        string url,
        string storageProvider,
        string? blobPath) : base(id,ownerId)
    {
        OwnerId = ownerId;
        OriginalFileName = originalFileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        Url = url;
        StorageProvider = storageProvider;
        BlobPath = blobPath;
    }

    public static StoredFile Create(
        UserId ownerId,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
#pragma warning disable CA1054
        string url,
#pragma warning restore CA1054
        string storageProvider,
        string? blobPath = null)
    {
        return new StoredFile(
            StoredFileId.New(),
            ownerId,
            originalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            url,
            storageProvider,
            blobPath);
    }
}
