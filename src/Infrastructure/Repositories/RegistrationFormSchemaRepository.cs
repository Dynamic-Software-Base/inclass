using Application.Abstractions.Interfaces.Repositories;
using Domain.Registrations;
using Domain.Schools;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Repositories;

public sealed class RegistrationFormSchemaRepository
    : IRegistrationFormSchemaRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationFormSchemaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationFormSchema?> GetBySchoolAndGradeAsync(
        SchoolId schoolId,
        GradeDefinitionId gradeDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RegistrationFormSchemas
            .FirstOrDefaultAsync(
                s => s.SchoolId == schoolId && s.GradeDefinitionId == gradeDefinitionId,
                cancellationToken);
    }

    public async Task<RegistrationFormSchema?> GetDefaultByGradeAsync(
        GradeDefinitionId gradeDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RegistrationFormSchemas
            .FirstOrDefaultAsync(
                s => s.SchoolId == null && s.GradeDefinitionId == gradeDefinitionId,
                cancellationToken);
    }

    public async Task AddAsync(
        RegistrationFormSchema schema,
        CancellationToken cancellationToken = default)
    {
        await _context.RegistrationFormSchemas.AddAsync(schema, cancellationToken);
    }

    public void Update(RegistrationFormSchema schema)
    {
        _context.RegistrationFormSchemas.Update(schema);
    }
}
