using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authentication;

public sealed class CurrentUser : ICurrentUser
{

    public static readonly CurrentUser Anonymous = new(
        UserId.From(Guid.Empty),
        string.Empty,
        "Anonymous",
        []
    );

    public CurrentUser(UserId userId, string email, string fullName, IReadOnlyList<UserRole> roles)
    {
        Id = userId;
        Email = email;
        FullName = fullName;
        Roles = roles;
        IsAuthenticated = Id.Value != Guid.Empty;
    }

    public UserId Id { get; }
    public string Email { get; }
    public string FullName { get; }
    public bool IsAuthenticated { get; }
    public IReadOnlyList<UserRole> Roles { get; }
    public bool IsInRole(UserRole role) => Roles.Contains(role);
}
