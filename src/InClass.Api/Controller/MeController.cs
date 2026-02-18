using Application.Schools.Contracts;
using Application.Schools.Queries.GetMySchools;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controller;

[ApiController]
[Route("api/me")]
public sealed class MeController : ControllerBase
{
    private readonly ISender _sender;

    public MeController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("schools")]
    [Authorize]
    public async Task<IActionResult> GetSchools(CancellationToken cancellationToken)
    {
        ErrorOr<List<MySchoolDto>> result = await _sender.Send(new GetMySchoolsQuery(), cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(response),
            errors => this.ToProblem(errors));
    }
}
