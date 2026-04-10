using Application.Abstractions.Messaging;
using Contract.InClass.Response.School.Classes;

namespace Application.Schools.Classes.Queries.GetSchoolClasses;

public record GetSchoolClassesQuery(Guid SchoolId,string AcademicYear)
    : IQuery<ErrorOr<List<SchoolClassResponse>>>;
