using Application.Abstractions.Interfaces.Repositories;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.StronglyTypedIds;
using SharedKernel.ValueObjects.StronglyTypedIds.SchoolClass;

namespace Infrastructure.Repositories;

public class SchoolClassRepository : ISchoolClassRepository
{
    private readonly ApplicationDbContext _context;

    public SchoolClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SchoolClass schoolClass, CancellationToken cancellationToken)
    {
        await _context.SchoolClasses.AddAsync(schoolClass,cancellationToken);
    }

    public async  Task AddRangeAsync(IEnumerable<SchoolClass> schoolClasses, CancellationToken cancellationToken)
    {
        await  _context.SchoolClasses.AddRangeAsync(schoolClasses, cancellationToken);
    }

    public async  Task<bool> ExistsAsync(SchoolId schoolId, GradeDefinitionId gradeDefinitionId, AcademicYear academicYear, string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.SchoolClasses
            .AsNoTracking()
            .AnyAsync(c =>
                    c.SchoolId == schoolId &&
                    c.GradeDefinitionId == gradeDefinitionId &&
                    c.AcademicYear == academicYear &&
                    c.Name.Value.Equals(name, StringComparison.OrdinalIgnoreCase),
                cancellationToken);

    }

    public async Task<SchoolClass?> GetByIdAsync(SchoolClassId id, CancellationToken cancellationToken = default)
    {
        return await  _context.SchoolClasses .FindAsync([id], cancellationToken);
    }
}
