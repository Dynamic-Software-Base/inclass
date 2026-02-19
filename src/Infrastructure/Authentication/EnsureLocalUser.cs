using Application.Abstractions.Authentication;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Authentication;

public class EnsureLocalUser : IEnsureLocalUserService
{

    private readonly ICurrentUserService _currentUser;
    private readonly ApplicationDbContext _db;

    public EnsureLocalUser(ApplicationDbContext db, ICurrentUserService currentUser)
    {
        this._db = db;
        this._currentUser = currentUser;
    }

    public async Task EnsureLocalUserAsync(CancellationToken cancellationToken = default)
    {
        ICurrentUser currentUser = _currentUser.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return;
        }
        bool exists = await _db.Set<User>()
            .AsNoTracking()
            .AnyAsync( u => u.Id == currentUser.Id, cancellationToken);

        if (exists)
        {
            return;
        }

        var user = User.Create(
            id: currentUser.Id,
            createdBy: currentUser.Id,
            fullName: currentUser.FullName ?? "User",
            email: string.IsNullOrWhiteSpace(currentUser.Email) ? null : currentUser.Email,
            phoneNumber: null,
            isActive: true
        );

        await _db.Set<User>().AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
