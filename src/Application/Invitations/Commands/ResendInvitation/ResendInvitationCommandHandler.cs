using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Invitations.Common;
using Application.Invitations.Contracts;
using Domain.Invitations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Enums;

namespace Application.Invitations.Commands.ResendInvitation;

public sealed class ResendInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<ResendInvitationCommand, ErrorOr<ResendInvitationResponse>>
{
    public async Task<ErrorOr<ResendInvitationResponse>> Handle(
        ResendInvitationCommand request,
        CancellationToken cancellationToken)
    {
        Invitation? invitation = await unitOfWork
            .Set<Invitation>()
            .FirstOrDefaultAsync(x => x.Id == request.InvitationId, cancellationToken);

        if (invitation is null)
        {
            return Error.NotFound("Invitation.Resend.NotFound", "Invitation was not found.");
        }

        if (invitation.SchoolId != request.SchoolId)
        {
            return Error.Forbidden(
                "Invitation.Resend.SchoolMismatch",
                "Invitation does not belong to this school.");
        }

        if (invitation.Status == InvitationStatus.Accepted)
        {
            return Error.Conflict(
                "Invitation.Resend.Accepted",
                "Accepted invitations cannot be resent.");
        }

        if (invitation.Status == InvitationStatus.Revoked)
        {
            return Error.Conflict(
                "Invitation.Resend.Revoked",
                "Revoked invitations cannot be resent.");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Error.Conflict(
                "Invitation.Resend.InvalidState",
                "Only pending invitations can be resent.");
        }

        string rawToken = InvitationTokenHasher.GenerateRawToken();
        string tokenHash = InvitationTokenHasher.ComputeHash(rawToken);
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        ICurrentUser currentUser = currentUserService.GetCurrentUser();

        invitation.Resend(
            tokenHash,
            utcNow.AddDays(7),
            utcNow,
            currentUser.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResendInvitationResponse(rawToken);
    }
}
