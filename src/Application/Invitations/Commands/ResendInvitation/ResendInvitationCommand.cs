using Application.Invitations.Contracts;
using Domain.Invitations;
using Domain.Schools;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.ResendInvitation;

public sealed record ResendInvitationCommand(
    SchoolId SchoolId,
    InvitationId InvitationId)
    : IRequest<ErrorOr<ResendInvitationResponse>>;
