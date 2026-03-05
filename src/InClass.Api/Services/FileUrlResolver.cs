using Application.Abstractions.Interfaces.Storage;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Web.Api.Services;

public class FileUrlResolver : IFileUrlResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileUrlResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
#pragma warning disable CA1055
    public string GetAccessUrl(StoredFileId storedFile)
#pragma warning restore CA1055
    {
        HttpRequest request = _httpContextAccessor.HttpContext?.Request;
        return $"{request?.Scheme}://{request?.Host}/api/files/{storedFile.Value}";
    }
}
