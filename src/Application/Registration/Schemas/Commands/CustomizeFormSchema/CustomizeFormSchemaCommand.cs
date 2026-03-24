using Application.Abstractions.Messaging;

namespace Application.Registration.Schemas.Commands.CustomizeFormSchema;

public record CustomizeFormSchemaCommand(
    Guid SchoolId,
    Guid GradeDefinitionId,
    string FormSchemaJson
    ): ICommand<ErrorOr<Guid>>;
