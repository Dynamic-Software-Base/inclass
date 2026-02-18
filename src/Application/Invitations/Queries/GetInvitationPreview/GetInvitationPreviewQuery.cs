using Application.Invitations.Contracts;
using MediatR;

namespace Application.Invitations.Queries.GetInvitationPreview;

public sealed record GetInvitationPreviewQuery(string Token) : IRequest<ErrorOr<InvitationPreviewResponse>>;
