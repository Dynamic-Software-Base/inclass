using Domain.Registrations;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Interfaces;

public interface IApplicationDbContext
{
    DbSet<RegistrationFormSchema>  RegistrationFormSchemas { get; }
    DbSet<School>  Schools { get; }
}
