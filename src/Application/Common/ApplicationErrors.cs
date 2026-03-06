namespace Application.Common;

public static class ApplicationErrors
{
    public static class StorageErrors
    {
        public static Error UploadFailed(string reason) => Error.Failure(
            code: "Storage.UploadFailed",
            description: $"Failed to upload file: {reason}");

        public static Error DownloadFailed(string reason) => Error.Failure(
            code: "Storage.DownloadFailed",
            description: $"Failed to download file: {reason}");

        public static Error DeleteFailed(string reason) => Error.Failure(
            code: "Storage.DeleteFailed",
            description: $"Failed to delete file: {reason}");

        public static Error FileNotFound => Error.NotFound(
            code: "Storage.FileNotFound",
            description: "The requested file was not found.");

        public static Error ProviderUnavailable(string provider) => Error.Failure(
            code: "Storage.ProviderUnavailable",
            description: $"Storage provider '{provider}' is not available.");

        public static Error AllProvidersUnavailable => Error.Failure(
            code: "Storage.AllProvidersUnavailable",
            description: "All storage providers are unavailable.");

        public static Error InvalidFile(string reason) => Error.Validation(
            code: "Storage.InvalidFile",
            description: reason);

        public static Error FileTooLarge(long maxSize) => Error.Validation(
            code: "Storage.FileTooLarge",
            description: $"File size exceeds the maximum allowed size of {maxSize / (1024 * 1024)}MB.");

        public static Error InvalidContentType(string contentType) => Error.Validation(
            code: "Storage.InvalidContentType",
            description: $"Content type '{contentType}' is not allowed.");

        public static Error InvalidExtension(string extension) => Error.Validation(
            code: "Storage.InvalidExtension",
            description: $"File extension '{extension}' is not allowed.");
    }
}
