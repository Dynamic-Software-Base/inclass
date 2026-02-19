using Application.Schools.Commands.CreateSchool;
using Application.Schools.Contracts;
using Application.Schools.Queries.GetSchoolMembers;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.ValueObjects.StronglyTypedIds;
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
            response => Created(
                $"/api/schools/{response.SchoolId.Value}",
                new { schoolId = response.SchoolId.Value, name = response.Name }),
            errors => this.ToProblem(errors));
    }

    [HttpGet("{schoolId:guid}/members")]
    [Authorize]
    public async Task<IActionResult> GetMembers(
        Guid schoolId,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<SchoolMemberDto>> result =
            await _sender.Send(new GetSchoolMembersQuery(SchoolId.From(schoolId)), cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(response),
            errors => this.ToProblem(errors));
    }
}
