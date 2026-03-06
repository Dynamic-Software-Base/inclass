using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Invitations.Common;
using Domain.Invitations;
using Domain.Schools;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<AcceptInvitationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return Error.Unauthorized("Invitation.Accept.Unauthorized", "Authentication is required to accept invitations.");
        }

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Error.Validation("Invitation.Token.Required", "Invitation token is required.");
        }

        string tokenHash;
        try
        {
            tokenHash = InvitationTokenHasher.ComputeHash(request.Token);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Invitation.Token.Invalid", "Invitation token is invalid.");
        }

        Invitation? invitation = await unitOfWork
            .Set<Invitation>()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (invitation is null)
        {
            return Error.NotFound("Invitation.Accept.NotFound", "Invitation was not found.");
        }

        User? user = await ResolveCurrentUserAsync(currentUser, cancellationToken);
        if (user is null)
        {
            return Error.NotFound("Invitation.Accept.UserNotFound", "Current user could not be resolved.");
        }

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        try
        {
            invitation.Accept(user, utcNow);
        }
        catch (InvalidOperationException exception)
        {
            if (invitation.Status == InvitationStatus.Expired)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return exception.Message.Contains("does not match", StringComparison.OrdinalIgnoreCase)
                ? Error.Forbidden("Invitation.Accept.TargetMismatch", exception.Message)
                : Error.Validation("Invitation.Accept.InvalidState", exception.Message);
        }

        bool membershipExists = await unitOfWork
            .Set<UserSchoolMembership>()
            .AnyAsync(
                x => x.UserId == user.Id &&
                     x.SchoolId == invitation.SchoolId &&
                     x.Role == invitation.Role,
                cancellationToken);

        if (!membershipExists)
        {
            var membership = UserSchoolMembership.Create(
                UserSchoolMembershipId.New(),
                user.Id,
                invitation.SchoolId,
                invitation.Role);

            await unitOfWork
                .Set<UserSchoolMembership>()
                .AddAsync(membership, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    private async Task<User?> ResolveCurrentUserAsync(ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        User? userByEntityId = await unitOfWork
            .Set<User>()
            .FirstOrDefaultAsync(x => x.Id == currentUser.Id, cancellationToken);

        if (userByEntityId is not null)
        {
            return userByEntityId;
        }

        string keycloakUserId = currentUser.Id.Value.ToString();

        return await unitOfWork
            .Set<User>()
            .FirstOrDefaultAsync(x => x.Id == UserId.From(keycloakUserId), cancellationToken);
    }
}
