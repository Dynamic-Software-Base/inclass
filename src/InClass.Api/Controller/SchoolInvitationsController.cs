using Application.Invitations.Commands.GenerateInvitation;
using Application.Invitations.Contracts;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
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
}
