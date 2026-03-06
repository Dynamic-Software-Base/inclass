using Contract.InClass.ApiContract;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controller;

[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    protected IActionResult ToApiResponse<T>(ErrorOr<T> result)
    {
        if (!result.IsError)
        {
            return Ok(ApiResponse<T>.Success(result.Value));
        }

        ApiError[] errors = result.Errors.Select(MapError).ToArray();

        int statusCode = 422;

        if (errors.Any(e => e.Type == ApiErrorType.System))
        {
            statusCode = 500;
        }
        else if (errors.Any(e => e.Type == ApiErrorType.UnAuthorized))
        {
            statusCode = 401;
        }
        else if (errors.Any(e => e.Type == ApiErrorType.NotFound))
        {
            statusCode = 404;
        }
        else if (errors.Any(e => e.Type == ApiErrorType.Conflict))
        {
            statusCode = 409;
        }

        return StatusCode(statusCode,ApiResponse<T>.Failure(errors));
    }

    private static ApiError MapError(Error error) => new()
    {
        Code = error.Code,
        Message = error.Description,
        Type = error.Type switch
        {
            ErrorType.Validation => ApiErrorType.Validation,
            ErrorType.NotFound => ApiErrorType.NotFound,
            ErrorType.Conflict => ApiErrorType.Conflict,
            _ => ApiErrorType.System
        }
    };
}
