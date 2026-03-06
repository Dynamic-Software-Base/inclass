using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Invitations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Enums;

namespace Application.Invitations.Commands.RevokeInvitation;

public sealed class RevokeInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<RevokeInvitationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RevokeInvitationCommand request,
        CancellationToken cancellationToken)
    {
        Invitation? invitation = await unitOfWork
            .Set<Invitation>()
            .FirstOrDefaultAsync(x => x.Id == request.InvitationId, cancellationToken);

        if (invitation is null)
        {
            return Error.NotFound("Invitation.Revoke.NotFound", "Invitation was not found.");
        }

        if (invitation.SchoolId != request.SchoolId)
        {
            return Error.Forbidden(
                "Invitation.Revoke.SchoolMismatch",
                "Invitation does not belong to this school.");
        }

        if (invitation.Status == InvitationStatus.Accepted)
        {
            return Error.Conflict(
                "Invitation.Revoke.Accepted",
                "Accepted invitations cannot be revoked.");
        }

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        ICurrentUser currentUser = currentUserService.GetCurrentUser();

        invitation.Revoke(utcNow, currentUser.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
