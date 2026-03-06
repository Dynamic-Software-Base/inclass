namespace Application.Files;

public sealed record FileDownloadResult(
    Stream Stream,
    string ContentType,
    string FileName);
