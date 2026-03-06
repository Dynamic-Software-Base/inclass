using Application.Schools.Commands.CreateSchool;
using Application.Schools.Contracts;
using Application.Schools.Queries.GetSchoolMembers;
using Application.Schools.Queries.GetSchools;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using ErrorOr;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.ValueObjects.StronglyTypedIds;
using Web.Api.Infrastructure;

namespace Web.Api.Controller;

[ApiController]
[Route("api/schools")]
public sealed class SchoolsController : ApiBaseController
{
    private readonly ISender _sender;

    public SchoolsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = SchoolPolicies.OwnerOrAdmin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSchoolCommand request,
        CancellationToken cancellationToken)
    {
        ErrorOr<CreateSchoolResponse> result =
            await _sender.Send(request, cancellationToken);

        return result.Match<IActionResult>(
            response => Created(
                $"/api/schools/{response.SchoolId.Value}",
                new { schoolId = response.SchoolId.Value, name = response.Name }),
            errors => this.ToProblem(errors));
    }



    [HttpGet("{schoolId:guid}/members")]
    [Authorize(Policy = SchoolPolicies.OwnerOrAdmin)]
    public async Task<IActionResult> GetMembers(
        Guid schoolId,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<SchoolMemberDto>> result =
            await _sender.Send(new GetSchoolMembersQuery(SchoolId.From(schoolId)), cancellationToken);

        return ToApiResponse(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetSchools(
        [FromQuery] GetSchoolsQuery request,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<PagedResult<SchoolSummaryDto>> result = await _sender.Send(request, cancellationToken);
        return ToApiResponse(result);
    }
}
