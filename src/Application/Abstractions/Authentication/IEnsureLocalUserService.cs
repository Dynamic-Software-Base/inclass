namespace Application.Abstractions.Authentication;

public interface IEnsureLocalUserService
{
    Task EnsureLocalUserAsync(CancellationToken cancellationToken = default);
}
