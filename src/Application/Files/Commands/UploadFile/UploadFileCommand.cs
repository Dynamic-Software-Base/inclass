using Application.Abstractions.Messaging;
using Contract.InClass.Response.Files;
using MediatR;

namespace Application.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Stream FileStream,
    string FileName,
    string ContentType) : ICommand<ErrorOr<UploadFileResult>>;
