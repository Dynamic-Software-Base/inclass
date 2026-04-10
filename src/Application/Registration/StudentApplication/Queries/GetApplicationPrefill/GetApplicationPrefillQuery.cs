using Application.Abstractions.Messaging;
using Contract.InClass.Response.Registration;

namespace Application.Registration.StudentApplication.Queries.GetApplicationPrefill;

public record GetApplicationPrefillQuery(
    string IdentityKey,
    Guid SessionId)
    : IQuery<ErrorOr<ApplicationPrefillResponse>>;
