using Application.Invitations.Commands.GenerateInvitation;
using Application.Invitations.Commands.ResendInvitation;
using Application.Invitations.Commands.RevokeInvitation;
using Application.Invitations.Contracts;
using ErrorOr;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;
using Web.Api.Infrastructure;

namespace Web.Api.Controller;

[ApiController]
[Route("api/schools/{schoolId:guid}/invitations")]
public sealed class SchoolInvitationsController : ControllerBase
{
    private readonly ISender _sender;

    public SchoolInvitationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = SchoolPolicies.OwnerOrAdmin)]
    public async Task<IActionResult> GenerateInvitation(
        Guid schoolId,
        [FromBody] GenerateInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new GenerateInvitationCommand(
            SchoolId.From(schoolId),
            request.Role,
            request.TargetType,
            request.TargetValue);

        ErrorOr<GenerateInvitationResponse> result = await _sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(response),
            errors => this.ToProblem(errors));
    }

    [HttpPost("{invitationId:guid}/revoke")]
    [Authorize(Policy = SchoolPolicies.OwnerOrAdmin)]
    public async Task<IActionResult> RevokeInvitation(
        Guid schoolId,
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        var command = new RevokeInvitationCommand(
            SchoolId.From(schoolId),
            InvitationId.From(invitationId));

        ErrorOr<Success> result = await _sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            errors => this.ToProblem(errors));
    }

    [HttpPost("{invitationId:guid}/resend")]
    [Authorize(Policy = SchoolPolicies.OwnerOrAdmin)]
    public async Task<IActionResult> ResendInvitation(
        Guid schoolId,
        Guid invitationId,
        CancellationToken cancellationToken)
    {
        var command = new ResendInvitationCommand(
            SchoolId.From(schoolId),
            InvitationId.From(invitationId));

        ErrorOr<ResendInvitationResponse> result = await _sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(response),
            errors => this.ToProblem(errors));
    }
}
