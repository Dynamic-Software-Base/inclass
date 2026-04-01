using Application.Abstractions.Interfaces.Repositories;
using Domain.Students;

namespace Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    public Task<Student?> GetByIdentityKeyAsync(string identityKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
