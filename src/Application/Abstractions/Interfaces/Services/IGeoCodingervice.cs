using SharedKernel.ValueObjects;

namespace Application.Abstractions.Interfaces.Services;

public interface IGeoCodingService
{
    Task<ErrorOr<Coordinates>> GetCoordinatesAsync(string fullAddress ,CancellationToken cancellationToken = default);
}
