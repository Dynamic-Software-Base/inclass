using FluentValidation;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionPeriod;
public class UpdateRegistrationSessionPeriodCommandValidator
    : AbstractValidator<UpdateRegistrationSessionPeriodCommand>
{
    public UpdateRegistrationSessionPeriodCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();

        RuleFor(x => x.OpenDate)
            .NotEmpty()
            .LessThan(x => x.CloseDate)
            .WithMessage("OpenDate must be before CloseDate.");

        RuleFor(x => x.CloseDate).NotEmpty();
    }
}
