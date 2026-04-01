using FluentValidation;

namespace Application.Schools.Classes.Commands.UpdateSchoolClassCapacity;

public class UpdateSchoolClassCapacityCommandValidator
    : AbstractValidator<UpdateSchoolClassCapacityCommand>
{
    public UpdateSchoolClassCapacityCommandValidator()
    {
        RuleFor(x => x.SchoolClassId).NotEmpty();
        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .LessThanOrEqualTo(60);
    }
}
