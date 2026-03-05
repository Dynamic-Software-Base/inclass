using Application.Files;
using Application.Files.Commands.UploadFile;
using Application.Files.Queries.GetFile;
using Application.Files.Queries.GetUserFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FileController : ControllerBase
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
        return result.Match<IActionResult>(
            value => Ok(value),
            errors => NotFound(errors)
        );

    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> ServeFile(Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<FileDownloadResult> result = await _sender.Send(new ServeFileQuery(id), cancellationToken);

        return result.Match<IActionResult>(
            file => File(file.Stream, file.ContentType, file.FileName, enableRangeProcessing: true),
            errors => errors[0].Type == ErrorType.NotFound
                ? NotFound(errors)
                : BadRequest(errors[0].Description));
    }
    [HttpGet("my-files")]
    public async Task<IActionResult> GetUserFiles(CancellationToken cancellationToken)
    {
        ErrorOr<List<UserFileResult>> result = await _sender.Send(new GetUserFilesQuery(), cancellationToken);

        return result.Match<IActionResult>(
            userFiles => Ok(userFiles),
            errors => BadRequest(errors));
    }
}
