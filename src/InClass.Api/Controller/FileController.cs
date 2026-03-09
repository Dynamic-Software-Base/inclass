using Application.Files;
using Application.Files.Commands.UploadFile;
using Application.Files.Queries.GetFile;
using Application.Files.Queries.GetUserFiles;
using Contract.InClass.Response.Files;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FileController : ApiBaseController
{
    private readonly ISender _sender;

    public FileController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        await using Stream stream = file.OpenReadStream();

        ErrorOr<UploadFileResult> result = await _sender.Send(
            new UploadFileCommand(stream, file.FileName, file.ContentType),
            cancellationToken);
        return ToApiResponse(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> ServeFile(Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<FileDownloadResult> result = await _sender.Send(new ServeFileQuery(id), cancellationToken);

        return result.Match<IActionResult>(
            file => File(file.Stream, file.ContentType, file.FileName, enableRangeProcessing: true),
            errors => ToApiResponse(ErrorOr<FileDownloadResult>.From(errors))
        );
    }
    [HttpGet("my-school-pictures")]
    public async Task<IActionResult> GetUserFiles(CancellationToken cancellationToken)
    {
        ErrorOr<List<UploadFileResult>> result = await _sender.Send(new GetUserFilesQuery(), cancellationToken);
        return ToApiResponse(result);
    }
}
