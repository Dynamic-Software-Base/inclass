using Application.Abstractions.Messaging;
using Contract.InClass.Response.Files;

namespace Application.Files.Queries.GetUserFiles;

public sealed record GetUserFilesQuery : IQuery<ErrorOr<List<UploadFileResult>>>;
