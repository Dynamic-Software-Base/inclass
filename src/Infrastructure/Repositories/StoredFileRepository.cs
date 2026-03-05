using Application.Abstractions.Interfaces.Repositories;
using Domain.File;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Repositories;

public class StoredFileRepository : IStorageFileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoredFileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<StoredFile?>> GetByIdAsync(StoredFileId fileId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
    }

    public async Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .Where(f => f.OwnerId == ownerId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async  Task<ErrorOr<bool>> ExistAsync(StoredFileId fileId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoredFiles
            .AnyAsync(f => f.Id == fileId, cancellationToken);
    }

    public async Task<ErrorOr<Success>> AddAsync(StoredFile file, CancellationToken cancellationToken = default)
    {
        await _dbContext.StoredFiles.AddAsync(file, cancellationToken);

        return Result.Success;
    }

    public void Delete(StoredFile file)
    {
        _dbContext.StoredFiles.Remove(file);
    }

    public async Task<ErrorOr<bool>> IsInUseAsync(StoredFileId file, CancellationToken cancellationToken = default)
    {

        bool usedInSchools = await _dbContext.Schools
            .AnyAsync(h => h.Pictures.Any(p => p.StoredFileId == file), cancellationToken);

        return usedInSchools;

    }
}
