using Application.Abstractions.Interfaces.Storage;
using Application.Common;
using Application.Common.Settings.Storage;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Providers;

public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly ILogger<AzureBlobStorageProvider> _logger;
    private readonly AzureBlobSettings _settings;
    private BlobContainerClient? _blobContainer;
    private readonly BlobServiceClient? _blobServiceClient;

    public AzureBlobStorageProvider(ILogger<AzureBlobStorageProvider> logger , IOptions<StorageSettings> options)
    {
        _logger = logger;
        _settings = options.Value.AzureBlob;

        if (!string.IsNullOrEmpty(_settings.ConnectionString))
        {
            try
            {
                _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[AzureBlobStorageProvider]Failed to initialize Azure Blob Storage client");
            }
        }
    }

    public string ProviderName => "AzureBlob";

    public async Task<ErrorOr<StorageUploadResult>> UploadAsync(Stream fileStream, string fileName, string contentType, string containerPath,
        CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            string storedFileName = StoredFileHelpers.GenerateStoredFileName(fileName);
            string blobPath = string.IsNullOrEmpty(containerPath)
                ? storedFileName
                : $"{containerPath.Trim('/')}/{storedFileName}";

            BlobClient blobClient = _blobContainer!.GetBlobClient(blobPath);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };
            await blobClient.UploadAsync(fileStream, options, cancellationToken);

            string url = !string.IsNullOrEmpty(_settings.BaseUrl)
                ? $"{_settings.BaseUrl.TrimEnd('/')}/{blobPath}"
                : blobClient.Uri.ToString();

            _logger.LogInformation(
                "Successfully uploaded file {FileName} to Azure Blob Storage at {BlobPath}",
                fileName, blobPath);

            return new StorageUploadResult(url, storedFileName, blobPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to Azure Blob Storage", fileName);
            return ApplicationErrors.StorageErrors.UploadFailed(ex.Message);
        }
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            BlobClient blobClient = _blobContainer!.GetBlobClient(blobPath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                return ApplicationErrors.StorageErrors.FileNotFound;
            }

            Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {BlobPath} from Azure Blob Storage", blobPath);
            return ApplicationErrors.StorageErrors.DownloadFailed(ex.Message);
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            BlobClient blobClient = _blobContainer!.GetBlobClient(blobPath);
            Response<bool> response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (!response.Value)
            {
                _logger.LogWarning("File {BlobPath} was not found in Azure Blob Storage", blobPath);
            }

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {BlobPath} from Azure Blob Storage", blobPath);
            return ApplicationErrors.StorageErrors.DeleteFailed(ex.Message);
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            _logger.LogWarning("Azure Blob Storage client is null - no connection string configured");
            return false;
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Azure Blob Storage is not available. Exception: {Message}",
                ex.Message);
            return false;
        }
    }

    private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken = default)
    {
        if (_blobContainer is not null)
        {
            return;
        }

        _blobContainer = _blobServiceClient!.GetBlobContainerClient(_settings.ContainerName);
        if (_settings.CreateContainerIfNoExists)
        {
            await _blobContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        }
    }
}
