using Application.Files;
using Domain.File;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Interfaces.Storage;

public interface IStorageService
{
    Task<ErrorOr<StoredFile>> UploadAsync(Stream fileStream,
        string fileName,
        string contentType,
        UserId ownerId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Stream>> DownloadAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Deleted>> DeleteAsync(StoredFileId fileId, CancellationToken cancellationToken = default);

    Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerAsync(UserId ownerId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<FileDownloadResult>> GetFileStreamAsync(StoredFileId fileId, CancellationToken cancellationToken = default);
}
