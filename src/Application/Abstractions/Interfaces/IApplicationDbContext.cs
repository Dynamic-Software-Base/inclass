using Domain.EducationalSystem.Entities;
using Domain.Registrations;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Interfaces;


public interface IApplicationDbContext
{
    DbSet<RegistrationFormSchema> RegistrationFormSchemas { get; }
    DbSet<School> Schools { get; }
    DbSet<RegistrationSession> RegistrationSessions { get; }
    DbSet<SchoolClass> SchoolClasses { get; }
    DbSet<StudentApplication> StudentApplications { get; }
    DbSet<GradeDefinition> GradeDefinitions { get; }
    DbSet<GradeCycleDefinition> GradeCycleDefinitions { get; }
}
