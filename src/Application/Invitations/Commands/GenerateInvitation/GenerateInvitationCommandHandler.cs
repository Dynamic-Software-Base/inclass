using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Invitations.Common;
using Application.Invitations.Contracts;
using Domain.Invitations;
using Domain.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.GenerateInvitation;

public sealed class GenerateInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<GenerateInvitationCommand, ErrorOr<GenerateInvitationResponse>>
{
    public async Task<ErrorOr<GenerateInvitationResponse>> Handle(
        GenerateInvitationCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = currentUserService.GetCurrentUser();

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
