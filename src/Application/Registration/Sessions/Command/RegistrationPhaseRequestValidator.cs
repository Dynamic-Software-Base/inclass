using Contract.InClass.Request.Registration.Session;
using FluentValidation;

namespace Application.Registration.Sessions.Command;

public class RegistrationPhaseRequestValidator : AbstractValidator<RegistrationPhaseRequest>
{
    public RegistrationPhaseRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThan(x => x.EndDate)
            .WithMessage("Phase StartDate must be before EndDate.");

        RuleFor(x => x.EndDate)
            .NotEmpty();

        RuleFor(x => x.PhaseType)
            .IsInEnum();

        RuleFor(x => x.AllowedApplicantType)
            .IsInEnum();
    }
}
