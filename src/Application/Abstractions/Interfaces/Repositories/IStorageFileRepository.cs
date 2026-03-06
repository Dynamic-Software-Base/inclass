using Domain.File;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IStorageFileRepository
{
    Task<ErrorOr<StoredFile?>> GetByIdAsync(StoredFileId fileId,CancellationToken cancellationToken = default);
    Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default);
    Task<ErrorOr<bool>> ExistAsync(StoredFileId fileId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AddAsync(StoredFile file, CancellationToken cancellationToken = default);
    void Delete(StoredFile file);


    Task<ErrorOr<bool>> IsInUseAsync(StoredFileId file, CancellationToken cancellationToken = default);
}
