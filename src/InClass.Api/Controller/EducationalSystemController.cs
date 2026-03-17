using Application.EducationalSystem.Queries.GetCyclesWithGrades;
using Application.EducationalSystem.Queries.GetDefaultEducationalSystem;
using Contract.InClass.Response.School.EducationalSystem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class EducationalSystemController : ApiBaseController
{
    private readonly ISender _sender;

    public EducationalSystemController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Returns all cycles with their grade definitions for a given educational system.
    /// Called by the school creation form to populate the curriculum picker.
    /// Defaults to the Moroccan MEN system when no ID is provided.
    /// </summary>
    [HttpGet("{id:guid}/cycles")]
    [Authorize]
    public async Task<IActionResult> GetCyclesWithGrades(
        Guid id,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<CycleResponse>> result = await _sender.Send(
            new GetCyclesWithGradesQuery(id),
            cancellationToken);

        return ToApiResponse(result);
    }

    /// <summary>
    /// Returns the default Moroccan MEN system ID and its cycles.
    /// Used on school creation to skip the system selection step entirely.
    /// </summary>
    [HttpGet("default")]
    [Authorize]
    public async Task<IActionResult> GetDefault(CancellationToken cancellationToken)
    {
        ErrorOr<EducationalSystemResponse> result = await _sender.Send(
            new GetDefaultEducationalSystemQuery(),
            cancellationToken);

        return ToApiResponse(result);
    }
}
