using Application.Invitations.Contracts;
using MediatR;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Invitations.Commands.GenerateInvitation;

public sealed record GenerateInvitationCommand(
    SchoolId SchoolId,
    UserRole Role,
    InvitationTargetType TargetType,
    string TargetValue)
    : IRequest<ErrorOr<GenerateInvitationResponse>>;
