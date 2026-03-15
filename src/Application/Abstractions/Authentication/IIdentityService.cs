using Domain.Schools;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Authentication;

public interface IIdentityService
{
    /// <summary>
    /// creates a user in keycloak and returns their UserId ( keycloak sub)
    /// used by platform admins to create school owner accounts
    /// </summary>
    Task<ErrorOr<string>> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        UserRole role,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Generates an invitation link for a user to register or login
    /// and be bound to a school
    /// </summary>
    Task<ErrorOr<string>> GenerateInvitationLinkAsync(
        string email,
        string firstName,
        string lastName,
        SchoolId schoolId,
        UserRole role,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Find if a user exist in keycloak by email
    /// </summary>
    Task<string?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<Success>> AssignRoleAsync(
        string keycloakUserId,
        UserRole role,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<Success>> DeactivateUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default
    );
}
