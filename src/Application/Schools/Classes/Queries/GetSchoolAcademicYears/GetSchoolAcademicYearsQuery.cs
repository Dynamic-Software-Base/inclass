using Application.Abstractions.Messaging;

namespace Application.Schools.Classes.Queries.GetSchoolAcademicYears;

public record GetSchoolAcademicYearsQuery(
    Guid SchoolId) : IQuery<ErrorOr<List<string>>>;
