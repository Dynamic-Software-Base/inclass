using FluentValidation;

namespace Application.Registration.Schemas.Commands.CustomizeFormSchema;

public class CustomizeFormSchemaCommandValidator : AbstractValidator<CustomizeFormSchemaCommand>
{
    public CustomizeFormSchemaCommandValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty();

        RuleFor(x => x.GradeDefinitionId)
            .NotEmpty();
        RuleFor(x => x.FormSchemaJson)
            .NotEmpty()
            .Must(BeValidJson)
            .WithMessage("SchemaJson must be valid JSON.");
    }

    private static bool BeValidJson(string json)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}
