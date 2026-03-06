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

        string normalizedTargetValue;
        try
        {
            normalizedTargetValue = Invitation.NormalizeTargetValue(request.TargetType, request.TargetValue);
        }
        catch (ArgumentException exception)
        {
            return Error.Validation("Invitation.Generate.InvalidTarget", exception.Message);
        }

        bool schoolExists = await unitOfWork
            .Set<School>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.SchoolId, cancellationToken);

        if (!schoolExists)
        {
            return Error.NotFound("Invitation.Generate.SchoolNotFound", "School was not found.");
        }

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        bool hasDuplicatePendingInvitation = await unitOfWork
            .Set<Invitation>()
            .AsNoTracking()
            .AnyAsync(
                invitation => invitation.SchoolId == request.SchoolId &&
                              invitation.Role == request.Role &&
                              invitation.TargetType == request.TargetType &&
                              invitation.TargetValue == normalizedTargetValue &&
                              invitation.Status == InvitationStatus.Pending &&
                              invitation.ExpiresAt > utcNow,
                cancellationToken);

        if (hasDuplicatePendingInvitation)
        {
            return Error.Conflict(
                "Invitation.DuplicatePending",
                "A pending invitation already exists for this target and role.");
        }

        string rawToken = InvitationTokenHasher.GenerateRawToken();
        string tokenHash = InvitationTokenHasher.ComputeHash(rawToken);

        var invitation = Invitation.Create(
            InvitationId.New(),
            currentUser.Id,
            request.SchoolId,
            request.Role,
            request.TargetType,
            normalizedTargetValue,
            tokenHash,
            utcNow.AddDays(7));

        await unitOfWork
            .Set<Invitation>()
            .AddAsync(invitation, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GenerateInvitationResponse(rawToken);
    }
}
