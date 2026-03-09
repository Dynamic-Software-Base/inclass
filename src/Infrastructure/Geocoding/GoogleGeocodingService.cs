using System.Text.Json;
using Application.Abstractions.Interfaces.Services;
using Application.Common.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.ValueObjects;

namespace Infrastructure.Geocoding;

public class GoogleGeocodingService : IGeoCodingService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleGeocodingOptions _options;
    private readonly ILogger<GoogleGeocodingService> _logger;

    public GoogleGeocodingService(HttpClient httpClient,   IOptions<GoogleGeocodingOptions> options, ILogger<GoogleGeocodingService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ErrorOr<Coordinates>> GetCoordinatesAsync(string fullAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            string url = $"{_options.BaseUrl}?address={Uri.EscapeDataString(fullAddress)}&key={_options.Apikey}";
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            GoogleGeocodingResponse? result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(json);

            if (result?.Status != "OK" || result.Results.Length == 0)
            {
                _logger.LogWarning("Geocoding returned no results for address: {Address}", fullAddress);
                return GeocodingErrors.NotFound;
            }

            GoogleLocation location = result.Results[0].Geometry.Location;
            return Coordinates.Create(location.Lat, location.Lng);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geocoding failed for address: {Address}", fullAddress);
            return GeocodingErrors.ServiceUnavailable;
        }
    }
}
