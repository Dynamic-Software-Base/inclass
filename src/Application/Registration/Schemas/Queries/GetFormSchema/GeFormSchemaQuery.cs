using Application.Abstractions.Messaging;
using Contract.InClass.Response.Registration.Schemas;

namespace Application.Registration.Schemas.Queries.GetFormSchema;

public record GetFormSchemaQuery(Guid SchoolId,Guid GradeDefinition):IQuery<ErrorOr<FormSchemaResponse>>;
