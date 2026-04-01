using FluentValidation;

namespace Application.Registration.Sessions.Command.UpdateRegistrationSessionCapacity;
public class UpdateRegistrationSessionCapacityCommandValidator
    : AbstractValidator<UpdateRegistrationSessionCapacityCommand>
{
    public UpdateRegistrationSessionCapacityCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.MaxSlots).GreaterThan(0);
    }
}
