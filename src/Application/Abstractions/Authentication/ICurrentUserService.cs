using SharedKernel;

namespace Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    ICurrentUser GetCurrentUser();
}
