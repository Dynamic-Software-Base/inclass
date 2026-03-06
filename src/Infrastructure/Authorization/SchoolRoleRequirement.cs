using Microsoft.AspNetCore.Authorization;
using SharedKernel.Enums;

namespace Infrastructure.Authorization;

public sealed class SchoolRoleRequirement : IAuthorizationRequirement
{
    public SchoolRoleRequirement(params UserRole[] allowedRoles)
    {
        ArgumentNullException.ThrowIfNull(allowedRoles);

        AllowedRoles = allowedRoles
            .Distinct()
            .ToArray();

        if (AllowedRoles.Length == 0)
        {
            throw new ArgumentException("At least one role must be specified.", nameof(allowedRoles));
        }
    }

    public UserRole[] AllowedRoles { get; }
}
