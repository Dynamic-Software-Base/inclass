using Application.Invitations.Commands.AcceptInvitation;
using Application.Invitations.Queries.GetInvitationPreview;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controller;

[ApiController]
[Route("api/invitations")]
public sealed class InvitationsController : ControllerBase
{
    private readonly ISender _sender;

    public InvitationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPreview(string token, CancellationToken cancellationToken)
    {
        ErrorOr<Application.Invitations.Contracts.InvitationPreviewResponse> result =
            await _sender.Send(new GetInvitationPreviewQuery(token), cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(response),
            errors => this.ToProblem(errors));
    }

    [HttpPost("{token}/accept")]
    [Authorize]
    public async Task<IActionResult> Accept(string token, CancellationToken cancellationToken)
    {
        ErrorOr<Success> result = await _sender.Send(new AcceptInvitationCommand(token), cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            errors => this.ToProblem(errors));
    }
}
