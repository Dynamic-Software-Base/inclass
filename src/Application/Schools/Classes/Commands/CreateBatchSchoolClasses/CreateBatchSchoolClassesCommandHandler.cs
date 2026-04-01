using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Domain.Schools.Enums;
using Domain.Schools.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Application.Schools.Classes.Commands.CreateBatchSchoolClasses;

public sealed class CreateBatchSchoolClassesCommandHandler
    : IRequestHandler<CreateBatchSchoolClassesCommand, ErrorOr<List<Guid>>>
{
    private readonly ISchoolClassRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBatchSchoolClassesCommandHandler(
        ISchoolClassRepository repository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _context = context;
        _currentUser = currentUser;
    }

    public async  Task<ErrorOr<List<Guid>>> Handle(CreateBatchSchoolClassesCommand request, CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);
        var gradeId = GradeDefinitionId.From(request.GradeDefinitionId);
        UserId userId = _currentUser.GetCurrentUser().Id;
        bool schoolExists = await _context.Schools
            .AnyAsync(s => s.Id == schoolId, cancellationToken);

        if (!schoolExists)
        {
            return Error.NotFound("SchoolClass.SchoolNotFound",
                "The specified school was not found.");
        }

        ErrorOr<AcademicYear> academicYearResult = AcademicYear.Create(request.AcademicYear);
        if (academicYearResult.IsError)
        {
            return academicYearResult.Errors;
        }
        var requestedNames = request.Classes.Select(c => c.Name).ToList();
        List<string> existingNames = await _context.SchoolClasses.Where(c => c.SchoolId == schoolId

                                                                             && c.GradeDefinitionId == gradeId
                                                                             && c.AcademicYear ==
                                                                             academicYearResult.Value
                                                                             &&
                                                                             requestedNames .Contains(c.Name.Value))
            .Select(c => c.Name.Value)
            .ToListAsync(cancellationToken);

        if (existingNames.Count > 0)
        {
            return Error.Conflict("SchoolClass.AlreadyExists",
                $"The following class names already exist: {string.Join(", ", existingNames)}.");
        }
        var schoolClasses = new List<SchoolClass>();
        foreach (SchoolClassDefinitionDto definition in request.Classes)
        {
            ErrorOr<ClassName> nameResult = ClassName.Create(definition.Name);
            if (nameResult.IsError){ return nameResult.Errors;}

            ErrorOr<ClassCapacity> capacityResult = ClassCapacity.Create(definition.MaxStudents);
            if (capacityResult.IsError)
            {
                return capacityResult.Errors;
            }

            ErrorOr<SchoolClass> classResult = SchoolClass.Create(
                SchoolClassId.New(),
                schoolId,
                gradeId,
                academicYearResult.Value,
                nameResult.Value,
                capacityResult.Value,
                userId);

            if (classResult.IsError)
            {
                return classResult.Errors;
            }

            schoolClasses.Add(classResult.Value);
        }

        await _repository.AddRangeAsync(schoolClasses, cancellationToken);

        return schoolClasses.Select(c => c.Id.Value).ToList();
    }
}
