using Application.Registration.Sessions.Command.CancelRegistrationSession;
using Application.Registration.Sessions.Command.CloseRegistrationSession;
using Application.Registration.Sessions.Command.CreateRegistratinSession;
using Application.Registration.Sessions.Command.OpenRegistrationSession;
using Application.Registration.Sessions.Command.UpdateRegistrationSessionPeriod;
using Application.Registration.Sessions.Command.UpdateRegistrationSessionQuota;
using Application.Registration.Sessions.Command.UpdateRegistrationSessionStrategy;
using Application.Registration.Sessions.Queries.GetEnrollmentSessionsForSchool;
using Application.Registration.Sessions.Queries.GetFormSchemaForSession;
using Application.Registration.Sessions.Queries.GetRegistrationSessions;
using Application.Registration.StudentApplication.Command.SubmitApplication;
using Application.Registration.StudentApplication.Queries.GetApplicationPrefill;
using Contract.InClass.Request.Registration.Application;
using Contract.InClass.Request.Registration.Session;
using Contract.InClass.Response.Registration;
using Contract.InClass.Response.Registration.Sessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistrationSessionsController : ApiBaseController
{
    private readonly ISender _sender;

    public RegistrationSessionsController(ISender sender)
    {
        _sender = sender;
    }

    // GET api/registrationsessions/{schoolId}?academicYear=2025-2026
    [HttpGet("{schoolId:guid}")]
    public async Task<IActionResult> GetSessions(Guid schoolId, [FromQuery] string academicYear)
    {
        ErrorOr<List<RegistrationSessionResponse>> result = await _sender.Send(new GetRegistrationSessionsQuery(schoolId, academicYear));
        return ToApiResponse(result);
    }

    // POST api/registrationsessions/batch
    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRegistrationSessionsCommand command)
    {
        ErrorOr<CreateBatchSessionsResult> result = await _sender.Send(command);
        return ToApiResponse(result);
    }

    // POST api/registrationsessions/{id}/open
    [HttpPost("{id:guid}/open")]
    public async Task<IActionResult> Open(Guid id)
    {
        ErrorOr<Success> result = await _sender.Send(new OpenRegistrationSessionCommand(id));
        return ToApiResponse(result);
    }

    // POST api/registrationsessions/{id}/close
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        ErrorOr<Success> result = await _sender.Send(new CloseRegistrationSessionCommand(id));
        return ToApiResponse(result);
    }

    // POST api/registrationsessions/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        ErrorOr<Success> result = await _sender.Send(new CancelRegistrationSessionCommand(id));
        return ToApiResponse(result);
    }

    // PATCH api/registrationsessions/{id}/period
    [HttpPatch("{id:guid}/period")]
    public async Task<IActionResult> UpdatePeriod(Guid id, [FromBody] UpdateRegistrationSessionPeriodCommand command)
    {
        if (id != command.SessionId)
        {
            return BadRequest();
        }

        ErrorOr<Success> result = await _sender.Send(command);
        return ToApiResponse(result);
    }

    // PATCH api/registrationsessions/{id}/quota
    [HttpPatch("{id:guid}/quota")]
    public async Task<IActionResult> UpdateQuota(Guid id, [FromBody] UpdateRegistrationSessionQuotaCommand command)
    {
        if (id != command.SessionId)
        {
            return BadRequest();
        }

        ErrorOr<Success> result = await _sender.Send(command);
        return ToApiResponse(result);
    }

    // PATCH api/registrationsessions/{id}/strategy
    [HttpPatch("{id:guid}/strategy")]
    public async Task<IActionResult> UpdateStrategy(Guid id, [FromBody] UpdateRegistrationSessionStrategyCommand command)
    {
        if (id != command.SessionId)
        {
            return BadRequest();
        }

        ErrorOr<Success> result = await _sender.Send(command);
        return ToApiResponse(result);
    }


    [HttpGet("{schoolId:guid}/enrollment-sessions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEnrollmentSessions(
        Guid schoolId,
        [FromQuery] string academicYear,
        CancellationToken cancellationToken)
    {
        ErrorOr<EnrollmentSessionsResponse> result = await _sender.Send(
            new GetEnrollmentSessionsForSchoolQuery(schoolId, academicYear),
            cancellationToken);

        return ToApiResponse(result);
    }

    [HttpGet("sessions/{sessionId:guid}/form-schema")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFormSchema(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        ErrorOr<FormSchemaResponse> result = await _sender.Send(
            new GetFormSchemaForSessionQuery(sessionId),
            cancellationToken);

        return ToApiResponse(result);
    }
    [HttpGet("applications/prefill")]
    [AllowAnonymous]
    public async Task<IActionResult> GetApplicationPrefill(
        [FromQuery] string identityKey,
        [FromQuery] Guid sessionId,
        CancellationToken cancellationToken)
    {
        ErrorOr<ApplicationPrefillResponse> result = await _sender.Send(
            new GetApplicationPrefillQuery(identityKey, sessionId),
            cancellationToken);

        return ToApiResponse(result);
    }

    [HttpPost("applications/submit")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitApplication(
        [FromBody] SubmitApplicationRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<SubmitApplicationResult> result = await _sender.Send(new SubmitApplicationCommand(
                request.SessionId,
                request.StudentFirstName,
                request.StudentLastName,
                request.ContactPhone,
                request.ContactEmail,
                request.IsReturning,
                request.IdentityKey,
                request.FormValuesJson),
            cancellationToken);

        return ToApiResponse(result);
    }
}
