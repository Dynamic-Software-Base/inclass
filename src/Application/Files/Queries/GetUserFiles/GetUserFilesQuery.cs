using Application.Abstractions.Messaging;

namespace Application.Files.Queries.GetUserFiles;

public sealed record GetUserFilesQuery : IQuery<ErrorOr<List<UserFileResult>>>;

public sealed record UserFileResult(
    Guid Id,
    string FileName,
#pragma warning disable CA1054
    string Url,
#pragma warning restore CA1054
    string ContentType,
    long SizeInBytes,
    DateTimeOffset UploadedAt);
