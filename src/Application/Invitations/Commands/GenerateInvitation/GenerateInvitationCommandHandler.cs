using Application.Abstractions.Authorization;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Invitations.Common;
using Application.Invitations.Contracts;
using Domain.Invitations;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.GenerateInvitation;

public sealed class GenerateInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    ISchoolAccessService schoolAccessService)
    : IRequestHandler<GenerateInvitationCommand, ErrorOr<GenerateInvitationResponse>>
{
    public async Task<ErrorOr<GenerateInvitationResponse>> Handle(
        GenerateInvitationCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            return Error.Unauthorized("Invitation.Generate.Unauthorized", "Authentication is required to generate invitations.");
        }

        if (string.IsNullOrWhiteSpace(request.TargetValue))
        {
            return Error.Validation("Invitation.Target.Required", "Invitation target value is required.");
        }

        bool schoolExists = await unitOfWork
            .Set<School>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.SchoolId, cancellationToken);

        if (!schoolExists)
        {
            return Error.NotFound("Invitation.Generate.SchoolNotFound", "School was not found.");
        }

        bool canManageInvitations = await schoolAccessService.HasAnyRoleAsync(
            currentUser.Id,
            request.SchoolId,
            [UserRole.SchoolOwner, UserRole.SchoolAdministrator],
            cancellationToken);

        if (!canManageInvitations)
        {
            return Error.Forbidden(
                "Invitation.Generate.Forbidden",
                "You are not allowed to generate invitations for this school.");
        }

        string rawToken = InvitationTokenHasher.GenerateRawToken();
        string tokenHash = InvitationTokenHasher.ComputeHash(rawToken);

        Invitation invitation;
        try
        {
            invitation = Invitation.Create(
                InvitationId.New(),
                currentUser.Id,
                request.SchoolId,
                request.Role,
                request.TargetType,
                request.TargetValue,
                tokenHash,
                DateTimeOffset.UtcNow.AddDays(7));
        }
        catch (ArgumentException exception)
        {
            return Error.Validation("Invitation.Generate.InvalidTarget", exception.Message);
        }

        await unitOfWork
            .Set<Invitation>()
            .AddAsync(invitation, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GenerateInvitationResponse(rawToken);
    }
}
