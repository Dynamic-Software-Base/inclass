using FluentValidation;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassName;

public class UpdateSchoolClassNameCommandValidator
    : AbstractValidator<UpdateSchoolClassNameCommand>
{
    public UpdateSchoolClassNameCommandValidator()
    {
        RuleFor(x => x.SchoolClassId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20);
    }
}
