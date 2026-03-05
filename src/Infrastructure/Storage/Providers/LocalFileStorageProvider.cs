using Application.Abstractions.Interfaces.Storage;
using Application.Common;
using Application.Common.Settings.Storage;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Providers;

public class LocalFileStorageProvider : IStorageProvider
{
    private readonly LocalStorageSettings _settings;
    private readonly ILogger<LocalFileStorageProvider> _logger;
    private readonly string _basePath;

    public LocalFileStorageProvider(ILogger<LocalFileStorageProvider> logger, IOptions<StorageSettings> options)
    {
        _logger = logger;
        _settings = options.Value.LocalStorage;

        _basePath = Path.IsPathRooted(_settings.BasePath)
            ? _settings.BasePath
            : Path.Combine(Directory.GetCurrentDirectory(), _settings.BasePath);
    }

    public string ProviderName => "LocalFile";

    public async Task<ErrorOr<StorageUploadResult>> UploadAsync(Stream fileStream, string fileName, string contentType, string containerPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string storedFileName = StoredFileHelpers.GenerateStoredFileName(fileName);

            string directoryPath = string.IsNullOrEmpty(containerPath)
                ? _basePath
                : Path.Combine(_basePath, containerPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, storedFileName);
            string relativePath = Path.GetRelativePath(_basePath, filePath).Replace("\\","/");

            await using var fileStreamOut = new FileStream(filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true
            );
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

            string url = $"{_settings.BaseUrl.TrimEnd('/')}/{relativePath}";
            _logger.LogInformation(
                "[LocalFileProvider]_[SaveFile] Successfully uploaded file {FileName} to local storage at {FilePath}",
                fileName, filePath);

            return new StorageUploadResult(url, storedFileName, relativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to local storage", fileName);
            return ApplicationErrors.StorageErrors.UploadFailed(ex.Message);
        }
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        try
        {
            string filePath = Path.Combine(_basePath, blobPath);

            if (!File.Exists(filePath))
            {
                return await Task.FromResult(ApplicationErrors.StorageErrors.FileNotFound);
            }

            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync:true);

            return await Task.FromResult<ErrorOr<Stream>>((Stream)stream);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {BlobPath} from local storage", blobPath);
            return await Task.FromResult<ErrorOr<Stream>>(ApplicationErrors.StorageErrors.DownloadFailed(ex.Message));
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        try
        {
            string filePath = Path.Combine(_basePath, blobPath);
            if (!File.Exists(filePath))
            {
                _logger.LogInformation("[LocalFileProvider] File {FilePath} was not found in local storage", filePath);
                return await Task.FromResult(ApplicationErrors.StorageErrors.FileNotFound);

            }
            File.Delete(filePath);
            _logger.LogInformation("[LocalFileProvider] Deleted file {FilePath} from local storage", filePath);
            return await Task.FromResult(new Deleted());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {BlobPath} from local storage", blobPath);
            return await Task.FromResult<ErrorOr<Deleted>>(ApplicationErrors.StorageErrors.DeleteFailed(ex.Message));
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            string testFile = Path.Combine(_basePath, ".write_test");
            await File.WriteAllTextAsync(testFile,"test",cancellationToken);
            File.Delete(testFile);

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,"[LocalFileStorage] local file storage is not available {BasePath}", _basePath);
            return await Task.FromResult(false);
        }
    }


}
