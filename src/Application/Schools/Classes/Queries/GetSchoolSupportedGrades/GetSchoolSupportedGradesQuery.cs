using Application.Abstractions.Messaging;
using Contract.InClass.Response.School.Classes;
using Domain.Schools.Entities;

namespace Application.Schools.Classes.Queries.GetSchoolSupportedGrades;

public record GetSchoolSupportedGradesQuery(Guid SchoolId) : IQuery<ErrorOr<SchoolSupportedGradesResponse>>;
