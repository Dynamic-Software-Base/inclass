namespace Application.Common.Settings.Storage;

public class FileValidationSettings
{
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;

    public string[] AllowedContentTypes { get; set; } =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    public string[] AllowedExtensions { get; set; } =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    ];
}
