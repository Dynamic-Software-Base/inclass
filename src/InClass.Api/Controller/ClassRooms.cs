using Application.Schools.Classes.Commands.CreateBatchSchoolClasses;
using Application.Schools.Classes.Commands.CreateSchoolClass;
using Application.Schools.Classes.Commands.UpdateSchoolClassCapacity;
using Application.Schools.Classes.Commands.UpdateSchoolClassName;
using Application.Schools.Classes.Queries.GetSchoolAcademicYears;
using Application.Schools.Classes.Queries.GetSchoolClasses;
using Application.Schools.Classes.Queries.GetSchoolSupportedGrades;
using Contract.InClass.Request.School.Classes;
using Contract.InClass.Response.School.Classes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassRooms : ApiBaseController
{
    private readonly ISender _sender;

    public ClassRooms(ISender sender)
    {
        _sender = sender;
    }

    // GET api/classrooms/{schoolId}/supported-grades
    [HttpGet("{schoolId:guid}/supported-grades")]
    public async Task<IActionResult> GetSupportedGrades(Guid schoolId)
    {
        ErrorOr<SchoolSupportedGradesResponse> result = await _sender.Send(new GetSchoolSupportedGradesQuery(schoolId));
        return ToApiResponse(result);
    }

    // GET api/classrooms/{schoolId}/academic-years
    [HttpGet("{schoolId:guid}/academic-years")]
    public async Task<IActionResult> GetAcademicYears(Guid schoolId)
    {
        ErrorOr<List<string>> result = await _sender.Send(new GetSchoolAcademicYearsQuery(schoolId));
        return ToApiResponse(result);
    }

    // GET api/classrooms/{schoolId}/classes?academicYear=2025-2026
    [HttpGet("{schoolId:guid}/classes")]
    public async Task<IActionResult> GetClasses(Guid schoolId, [FromQuery] string academicYear)
    {
        ErrorOr<List<SchoolClassResponse>> result = await _sender.Send(new GetSchoolClassesQuery(schoolId, academicYear));
        return ToApiResponse(result);
    }

    // POST api/classrooms/{schoolId}/classes
    [HttpPost("{schoolId:guid}/classes")]
    public async Task<IActionResult> CreateClass(Guid schoolId,
        [FromBody] CreateSchoolClassRequest request)
    {
        ErrorOr<Guid> result = await _sender.Send(new CreateSchoolClassCommand(
            request.SchoolId,
            request.GradeDefinitionId,
            request.Name,
            request.AcademicYear,
            request.MaxStudents));
        return ToApiResponse(result);
    }

    // POST api/classrooms/{schoolId}/classes/batch
    [HttpPost("{schoolId:guid}/classes/batch")]
    public async Task<IActionResult> CreateBatchClasses(
        Guid schoolId, [FromBody] CreateBatchSchoolClassesRequest request)
    {
        ErrorOr<List<Guid>> result = await _sender.Send(new CreateBatchSchoolClassesCommand(
            request.SchoolId,
            request.GradeDefinitionId,
            request.AcademicYear,
            request.Classes.Select(c  => new SchoolClassDefinitionDto(
                Name: c.Name,
                MaxStudents: c.MaxStudents
                )).ToList() ));
        return ToApiResponse(result);
    }

    // PATCH api/classrooms/classes/{classId}/name
    [HttpPatch("classes/{classId:guid}/name")]
    public async Task<IActionResult> UpdateClassName(Guid classId, [FromBody] UpdateSchoolClassNameRequest request)
    {
        ErrorOr<Success> result = await _sender.Send(new UpdateSchoolClassNameCommand(
            request.SchoolClassId,
            request.Name));
        return ToApiResponse(result);
    }

    // PATCH api/classrooms/classes/{classId}/capacity
    [HttpPatch("classes/{classId:guid}/capacity")]
    public async Task<IActionResult> UpdateClassCapacity(
        Guid classId, [FromBody] UpdateSchoolClassCapacityRequest request)
    {
        ErrorOr<Success> result = await _sender.Send(new UpdateSchoolClassCapacityCommand(
            request.SchoolClassId,
            request.MaxStudents));
        return ToApiResponse(result);
    }
}
