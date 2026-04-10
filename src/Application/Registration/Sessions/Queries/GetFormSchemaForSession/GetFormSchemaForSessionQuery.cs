using Application.Abstractions.Messaging;
using Contract.InClass.Response.Registration;

namespace Application.Registration.Sessions.Queries.GetFormSchemaForSession;

public record GetFormSchemaForSessionQuery(Guid SessionId)
    : IQuery<ErrorOr<FormSchemaResponse>>;
