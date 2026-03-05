using Application.Abstractions.Interfaces.Storage;
using Application.Common;
using Application.Common.Settings.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

public class FileValidator: IFileValidator
{
    private readonly FileValidationSettings _settings;

    public FileValidator(IOptions<StorageSettings> options)
    {
        _settings = options.Value.Validation;
    }
    public ErrorOr<Success> Validate(IFormFile file)
    {
        return Validate(file.FileName, file.ContentType, file.Length);
    }

    public ErrorOr<Success> Validate(string fileName, string contentType, long sizeInBytes)
    {
        var errors = new List<Error>();

        // Validate size
        if (sizeInBytes > _settings.MaxFileSizeBytes)
        {
            errors.Add(ApplicationErrors.StorageErrors.FileTooLarge(_settings.MaxFileSizeBytes));
        }

        // Validate content type
        if (!_settings.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(ApplicationErrors.StorageErrors.InvalidContentType(contentType));
        }

        // Validate extension
        string extension = Path.GetExtension(fileName);
        if (!_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(ApplicationErrors.StorageErrors.InvalidExtension(extension));
        }

        return errors.Count > 0 ? errors : Result.Success;
    }
}
