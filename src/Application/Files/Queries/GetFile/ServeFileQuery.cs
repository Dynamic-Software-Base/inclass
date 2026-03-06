using Application.Abstractions.Messaging;

namespace Application.Files.Queries.GetFile;

public record ServeFileQuery(Guid FileId):IPublicQuery<ErrorOr<FileDownloadResult>>;
