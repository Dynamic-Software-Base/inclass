using Application.Abstractions.Messaging;
using Contract.InClass.Response.Registration;

namespace Application.Registration.Sessions.Queries.GetEnrollmentSessionsForSchool;

public record GetEnrollmentSessionsForSchoolQuery(Guid SchoolId, string AcademicYear)
    : IQuery<ErrorOr<EnrollmentSessionsResponse>>;
