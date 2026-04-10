using Application.Abstractions.Messaging;
using Contract.InClass.Response.Registration.Sessions;

namespace Application.Registration.Sessions.Queries.GetRegistrationSessions;

public record GetRegistrationSessionsQuery(Guid SchoolId, string AcademicYear)
    : IQuery<ErrorOr<List<RegistrationSessionResponse>>>;
