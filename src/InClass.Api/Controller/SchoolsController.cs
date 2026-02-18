using Application.Schools.Commands.CreateSchool;
using Application.Schools.Contracts;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controller;

[ApiController]
[Route("api/schools")]
public sealed class SchoolsController : ControllerBase
{
    private readonly ISender _sender;

    public SchoolsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateSchoolRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<CreateSchoolResponse> result =
            await _sender.Send(new CreateSchoolCommand(request.Name), cancellationToken);

        return result.Match<IActionResult>(
            response => Created($"/api/schools/{response.SchoolId}", response),
            errors => this.ToProblem(errors));
    }
}
