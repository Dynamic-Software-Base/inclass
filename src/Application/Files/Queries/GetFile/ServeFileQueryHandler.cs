using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using MediatR;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Files.Queries.GetFile;

public class ServeFileQueryHandler: IRequestHandler<ServeFileQuery,ErrorOr<FileDownloadResult>>
{
    private readonly IStorageService _fileService;

    public ServeFileQueryHandler(IStorageService fileService)
    {
        _fileService = fileService;
    }

    public async Task<ErrorOr<FileDownloadResult>> Handle(ServeFileQuery request, CancellationToken cancellationToken)
    {
        var fileId = (StoredFileId)request.FileId;
        return await _fileService.GetFileStreamAsync(fileId, cancellationToken);
    }
}
