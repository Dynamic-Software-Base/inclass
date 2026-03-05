using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Stream FileStream,
    string FileName,
    string ContentType) : ICommand<ErrorOr<UploadFileResult>>;


public sealed record UploadFileResult(
    Guid Id,
    string FileName,
#pragma warning disable CA1054
    string Url,
#pragma warning restore CA1054
    string ContentType,
    long SizeInBytes
    );
