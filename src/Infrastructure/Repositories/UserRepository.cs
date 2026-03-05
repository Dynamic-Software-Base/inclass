using Application.Abstractions.Interfaces.Repositories;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
       User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id ==  identityId, cancellationToken);
       return user;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _db.Users.AddAsync(user, cancellationToken);
    }
}
