using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SchoolRepository : ISchoolRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SchoolRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async  Task<ErrorOr<Success>> AddAsync(School school, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(school, cancellationToken);
        return Result.Success;
    }

    public async Task<ErrorOr<bool>> ExistAsync(string name, CancellationToken cancellationToken = default)
    {
       return await _dbContext.Schools.AnyAsync(s => s.Name.Trim() == name.Trim(),cancellationToken);
    }
}
