using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Application.Common;
using Application.Common.Settings.Storage;
using Application.Files;
using Domain.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Storage;

public class FileStorageService : IStorageService
{
    private readonly IStorageProvider _primaryProvider;
    private readonly IStorageProvider _fallbackProvider;
    private readonly IStorageFileRepository _fileRepository;
    private readonly IFileValidator _fileValidator;
    private readonly ILogger<FileStorageService> _logger;
    private readonly StorageSettings _settings;

    public FileStorageService(IEnumerable<IStorageProvider> storageProviders,IOptions<StorageSettings> settings,ILogger<FileStorageService> logger
    , IStorageFileRepository fileRepository, IFileValidator fileValidator)
    {
        _fileRepository = fileRepository;
        _fileValidator = fileValidator;
        _logger = logger;
        _settings = settings.Value;

        var providerList = storageProviders.ToList();
        _primaryProvider = _settings.PrimaryProvider.Equals("AzureBlob", StringComparison.OrdinalIgnoreCase)
            ? providerList.First(p => p.ProviderName == "AzureBlob")
            : providerList.First(p => p.ProviderName == "LocalFile");

        _fallbackProvider = _primaryProvider.ProviderName == "AzureBlob"
            ? providerList.First(p => p.ProviderName == "LocalFile")
            : providerList.First(p => p.ProviderName == "AzureBlob");
    }
    public async Task<ErrorOr<StoredFile>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        UserId ownerId,
        CancellationToken cancellationToken = default)
    {

        // Validate file
        ErrorOr<Success> validationResult = _fileValidator.Validate(fileName, contentType, fileStream.Length);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }

        long fileSize = fileStream.Length;

        // Generate container path based on owner and date
        string containerPath = $"{ownerId.Value:N}/{DateTime.UtcNow:yyyy/MM}";

        // Try primary provider
        ErrorOr<(string url,string storedFileName,string blobPath, string providerName)> uploadResult = await TryUploadAsync(
            _primaryProvider,
            fileStream,
            fileName,
            contentType,
            containerPath,
            cancellationToken);

        // Fallback if enabled and primary failed
        if (uploadResult.IsError && _settings.EnableFallback)
        {
            _logger.LogWarning(
                "Primary storage provider {Provider} failed, falling back to {FallbackProvider}",
                _primaryProvider.ProviderName,
                _fallbackProvider.ProviderName);

            // Reset stream position
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            uploadResult = await TryUploadAsync(
                _fallbackProvider,
                fileStream,
                fileName,
                contentType,
                containerPath,
                cancellationToken);
        }

        if (uploadResult.IsError)
        {
            return uploadResult.Errors;
        }

#pragma warning disable IDE0008
        var (url, storedFileName, blobPath, providerName) = uploadResult.Value;
#pragma warning restore IDE0008

        // Create StoredFile entity
        ErrorOr<StoredFile> storedFile = StoredFile.Create(
            ownerId,
            fileName,
            storedFileName,
            contentType,
            fileSize,
            url,
            providerName,
            blobPath);
        if (storedFile.IsError)
        {
            return  storedFile.Errors;
        }
        await _fileRepository.AddAsync(storedFile.Value, cancellationToken);
        _logger.LogInformation(
            "File {FileName} uploaded successfully to {Provider}. StoredFileId: {StoredFileId}",
            fileName, providerName, storedFile.Value.Id);

        return storedFile;
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<StoredFile?> storedFileResult = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFileResult.IsError)
        {
            return storedFileResult.Errors;
        }
        StoredFile? storedFile = storedFileResult.Value;
        if (storedFile is null)
        {
            return ApplicationErrors.StorageErrors.FileNotFound;
        }

        IStorageProvider? provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is null)
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(storedFile.StorageProvider);
        }

        return await provider.DownloadAsync(storedFile.BlobPath!, cancellationToken);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<StoredFile?> storedFileResult = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFileResult.IsError)
        {
            return storedFileResult.Errors;
        }
        StoredFile? storedFile = storedFileResult.Value;
        if (storedFile is null)
        {
            return ApplicationErrors.StorageErrors.FileNotFound;
        }

        // Check if file is in use
        ErrorOr<bool> isInUseResult = await _fileRepository.IsInUseAsync(fileId, cancellationToken);
        if (isInUseResult.IsError)
        {
            return isInUseResult.Errors;
        }

        if (isInUseResult.Value)
        {
            return Error.Conflict(
                code: "Storage.FileInUse",
                description: "Cannot delete file because it is still in use.");
        }

        IStorageProvider? provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is not null && !string.IsNullOrEmpty(storedFile.BlobPath))
        {
            ErrorOr<Deleted> deleteResult = await provider.DeleteAsync(storedFile.BlobPath, cancellationToken);
            if (deleteResult.IsError)
            {
                _logger.LogWarning(
                    "Failed to delete file from storage provider, but will remove database record. Error: {Error}",
                    deleteResult.FirstError.Description);
            }
        }

        _fileRepository.Delete(storedFile);
        return Result.Deleted;
    }

    public async Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<IReadOnlyList<StoredFile>> files = await _fileRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (files.IsError)
        {
            return files.Errors;
        }
        return files.Value.ToList();
    }

    public async Task<ErrorOr<FileDownloadResult>> GetFileStreamAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<StoredFile?> storedFileResult = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFileResult.IsError)
        {
            return storedFileResult.Errors;
        }
        StoredFile? storedFile = storedFileResult.Value;
        if (storedFile is null)
        {
            return ApplicationErrors.StorageErrors.FileNotFound;
        }

        IStorageProvider? provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is null)
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(storedFile.StorageProvider);
        }


        ErrorOr<Stream> streamResult = await provider.DownloadAsync(storedFile.BlobPath!, cancellationToken);
        if (streamResult.IsError)
        {
            return streamResult.Errors;
        }


        return new FileDownloadResult(
            streamResult.Value,
            storedFile.ContentType,
            storedFile.OriginalFileName);
    }

    private async Task<ErrorOr<(string Url, string StoredFileName, string BlobPath, string ProviderName)>> TryUploadAsync(
        IStorageProvider provider,
        Stream fileStream,
        string fileName,
        string contentType,
        string containerPath,
        CancellationToken cancellationToken)
    {
        if (!await provider.IsAvailableAsync(cancellationToken))
        {
            return ApplicationErrors.StorageErrors.ProviderUnavailable(provider.ProviderName);
        }

        ErrorOr<StorageUploadResult> result = await provider.UploadAsync(
            fileStream,
            fileName,
            contentType,
            containerPath,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }


        return (result.Value.Url.ToString(), result.Value.StoredFileName, result.Value.BlobPath, provider.ProviderName);
    }

    private IStorageProvider? GetProviderByName(string providerName)
    {
        if (_primaryProvider.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
        {
            return _primaryProvider;
        }

        return _fallbackProvider.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase) ? _fallbackProvider : null;
    }
}
