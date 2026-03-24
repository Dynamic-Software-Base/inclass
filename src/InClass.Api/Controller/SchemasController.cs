using Application.Registration.Schemas.Commands.CustomizeFormSchema;
using Application.Registration.Schemas.Queries.GetFormSchema;
using Contract.InClass.Response.Registration.Schemas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;

[ApiController]
[Route("api/schools/{schoolId}/grades/{gradeDefinitionId}")]
[Authorize]
public class SchemasController : ApiBaseController
{
    private readonly ISender sender;

    public SchemasController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPut("schema")]
    public async Task<IActionResult> CustomizeSchema(
        Guid schoolId,
        Guid gradeDefinitionId,
        [FromBody] string schemaJson)
    {
        var command = new CustomizeFormSchemaCommand(schoolId, gradeDefinitionId, schemaJson);
        ErrorOr<Guid> result = await sender.Send(command);
        return ToApiResponse(result);
    }

    [HttpGet("schema")]
    public async Task<IActionResult> GetSchema(
        Guid schoolId,
        Guid gradeDefinitionId)
    {
        var query = new GetFormSchemaQuery(schoolId, gradeDefinitionId);
        ErrorOr<FormSchemaResponse> result = await sender.Send(query);
        return ToApiResponse(result);
    }
}
