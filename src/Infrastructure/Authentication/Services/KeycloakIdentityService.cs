using Application.Abstractions.Authentication;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authentication.Services;

public class KeycloakIdentityService : IIdentityService
{
    private readonly IKeycloakUserClient _userClient;
    private readonly IKeycloakRealmClient _realmClient;
    private readonly KeycloakSettings _settings;

    public KeycloakIdentityService(
        IKeycloakUserClient userClient,
        IKeycloakRealmClient realmClient,
        IOptions<KeycloakSettings> settings)
    {
        _userClient = userClient;
        _realmClient = realmClient;
        _settings = settings.Value;
    }

    public async Task<ErrorOr<string>> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            UserRepresentation userRepresentation = new()
            {
                Email = email,
                Username = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true,
                EmailVerified = true,
                Credentials =
                [
                    new()
                    {
                        Type = "password",
                        Value = GenerateTemporaryPassword(),
                        Temporary = true
                    }
                ]
            };

            HttpResponseMessage response = await _userClient.CreateUserWithResponseAsync(
                _settings.Realm,
                userRepresentation,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure("CreateUser.Failed", "Failed to create user in Keycloak");
            }

            string[]? segments = response.Headers.Location?.Segments;
            string? userId = segments is { Length: > 0 } ? segments[^1].Trim('/') : null;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Error.Failure("CreateUser.NoUserId", "User created but ID not returned");
            }

            ErrorOr<Success> assignRoleResult = await AssignRoleAsync(userId, role, cancellationToken);
            if (assignRoleResult.IsError)
            {
                return assignRoleResult.Errors;
            }

            return userId;
        }
        catch (Exception e)
        {
            return Error.Failure("CreateUser.Exception", e.Message);
        }
    }

    public async Task<ErrorOr<string>> GenerateInvitationLinkAsync(
        string email,
        string firstName,
        string lastName,
        SchoolId schoolId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        string? existingUserId = await FindUserByEmailAsync(email, cancellationToken);

        if (existingUserId is null)
        {
            ErrorOr<string> createResult = await CreateUserAsync(email, firstName, lastName, role, cancellationToken);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }
        }
        else
        {
            ErrorOr<Success> assignRoleResult = await AssignRoleAsync(existingUserId, role, cancellationToken);
            if (assignRoleResult.IsError)
            {
                return assignRoleResult.Errors;
            }
        }

        string invitationToken = Guid.NewGuid().ToString("N");
        string invitationLink =
            $"https://{_settings.AppName}/invitation/{invitationToken}?school={schoolId}&role={role}";

        return invitationLink;
    }

    public async Task<string?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<UserRepresentation> users = await _userClient.GetUsersAsync(
                _settings.Realm,
                new GetUsersRequestParameters { Email = email, Exact = true },
                cancellationToken
            );

            return users.FirstOrDefault()?.Id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ErrorOr<Success>> AssignRoleAsync(
        string keycloakUserId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string roleName = MapRoleToKeycloak(role);

            RealmRepresentation realm = await _realmClient.GetRealmAsync(
                _settings.Realm,
                cancellationToken
            );

            bool roleExists = realm.Roles?.Realm?.Any(
                realmRole => string.Equals(realmRole.Name, roleName, StringComparison.Ordinal)
            ) == true;

            if (!roleExists)
            {
                return Error.NotFound($"Role '{roleName}' not found in Keycloak");
            }

            UserRepresentation user = await _userClient.GetUserAsync(
                _settings.Realm,
                keycloakUserId,
                false,
                cancellationToken
            );

            List<string> userRealmRoles = user.RealmRoles?.ToList() ?? [];

            if (!userRealmRoles.Contains(roleName, StringComparer.Ordinal))
            {
                userRealmRoles.Add(roleName);
                user.RealmRoles = userRealmRoles;

                await _userClient.UpdateUserAsync(
                    _settings.Realm,
                    keycloakUserId,
                    user,
                    cancellationToken
                );
            }

            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("AssignRole.Failed", ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeactivateUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            UserRepresentation user = await _userClient.GetUserAsync(
                _settings.Realm,
                keycloakUserId,
                false,
                cancellationToken
            );

            if (string.IsNullOrWhiteSpace(user.Id))
            {
                return Error.NotFound("User not found");
            }

            user.Enabled = false;

            await _userClient.UpdateUserAsync(
                _settings.Realm,
                keycloakUserId,
                user,
                cancellationToken
            );

            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("DeactivateUser.Failed", ex.Message);
        }
    }

    private static string MapRoleToKeycloak(UserRole role) => role switch
    {
        UserRole.PlatformAdmin => "platform_admin",
        UserRole.SchoolOwner => "school_owner",
        UserRole.Teacher => "teacher",
        UserRole.Student => "student",
        UserRole.Parent => "parent",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };

    private static string GenerateTemporaryPassword()
    {
        return $"Temp{Guid.NewGuid():N}!";
    }
}
