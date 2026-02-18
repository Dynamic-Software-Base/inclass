using System.Security.Claims;
using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authentication.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private CurrentUser? _currentUser;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ICurrentUser GetCurrentUser()
    {
        if (_currentUser is not null)
        {
            return _currentUser;
        }

        ClaimsPrincipal user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return CurrentUser.Anonymous;
        }

        string id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        string email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        string name = user.FindFirstValue("name")
                      ?? $"{user.FindFirstValue("given_name")} {user.FindFirstValue("family_name")}";

        var roleClaims = user.FindAll(ClaimTypes.Role).
            Select(c => c.Value)
            .ToList();

        List<UserRole> roles = MapRoles(roleClaims);
        _currentUser = new CurrentUser(
            UserId.From(id!),
            email,
            name,
            roles
        );
        return _currentUser;
    }

    private static List<UserRole> MapRoles(List<string> keycloakRoles)
    {
        var roles = new List<UserRole>();

        foreach (string role in keycloakRoles)
        {
            UserRole? mapped = role.ToUpperInvariant() switch
            {
                "PLATFORM_ADMIN" => UserRole.PlatformAdmin,
                "SCHOOL-ADMINISTRATORS" => UserRole.SchoolAdministrator,
                "SCHOOL_OWNER" => UserRole.SchoolOwner,
                "TEACHER" => UserRole.Teacher,
                "STUDENT" => UserRole.Student,
                "PARENT" => UserRole.Parent,
                _ => (UserRole?)null
            };

            if (mapped.HasValue)
            {
                roles.Add(mapped.Value);
            }
        }

        return roles;
    }

}
