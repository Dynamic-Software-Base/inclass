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

namespace Application.Schools.Classes.Commands.CreateSchoolClass;

public sealed class CreateSchoolClassCommandHandler
    : IRequestHandler<CreateSchoolClassCommand, ErrorOr<Guid>>
{
    private readonly ISchoolClassRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateSchoolClassCommandHandler(ICurrentUserService currentUser, IApplicationDbContext context, ISchoolClassRepository repository)
    {
        _currentUser = currentUser;
        _context = context;
        _repository = repository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateSchoolClassCommand request, CancellationToken cancellationToken)
    {
        var schoolId = SchoolId.From(request.SchoolId);
        UserId userId = _currentUser.GetCurrentUser().Id;
        var gradedefinitionId = GradeDefinitionId.From(request.GradeDefinitionId);

        bool schoolExists = await _context.Schools.AnyAsync(s => s.Id == schoolId,cancellationToken);
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

        bool duplicate = await _repository.ExistsAsync(schoolId, gradedefinitionId, academicYearResult.Value,
            request.Name, cancellationToken);
        if (duplicate)
        {
            return Error.Conflict("SchoolClass.AlreadyExists",
                $"A class named '{request.Name}' already exists for this grade and academic year.");
        }

        ErrorOr<ClassName> classNameResult = ClassName.Create(request.Name);
        if (classNameResult.IsError)
        {
            return classNameResult.Errors;
        }

        ErrorOr<ClassCapacity> classCapacityResult = ClassCapacity.Create(request.MaxStudents);
        if (classCapacityResult.IsError)
        {
            return classCapacityResult.Errors;
        }

        ErrorOr<SchoolClass> classResult = SchoolClass.Create(
            SchoolClassId.New(),
            schoolId,
            gradedefinitionId,
            academicYearResult.Value,
            classNameResult.Value,
            classCapacityResult.Value,
            userId);
        if (classResult.IsError)
        {
            return classResult.Errors;
        }

        await _repository.AddAsync(classResult.Value, cancellationToken);
        return classResult.Value.Id.Value;
    }
}
