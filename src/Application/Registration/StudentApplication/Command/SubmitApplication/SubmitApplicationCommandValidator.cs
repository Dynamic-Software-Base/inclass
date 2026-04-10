using FluentValidation;

namespace Application.Registration.StudentApplication.Command.SubmitApplication;

public class SubmitApplicationCommandValidator
    : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty();

        RuleFor(x => x.StudentFirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StudentLastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ContactPhone)
            .NotEmpty();

        RuleFor(x => x.IdentityKey)
            .NotEmpty()
            .WithMessage("IdentityKey is required for returning students.")
            .When(x => x.IsReturning);

        RuleFor(x => x.FormValuesJson)
            .NotEmpty()
            .Must(BeValidJson)
            .WithMessage("FormValuesJson must be valid JSON.");
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
