using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Infrastructure;

internal static class ErrorOrProblemDetailsExtensions
{
    public static IActionResult ToProblem(this ControllerBase controller, List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return controller.Problem();
        }

        if (errors.TrueForAll(static error => error.Type == ErrorType.Validation))
        {
            var validationErrors = errors
                .GroupBy(static error => error.Code)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.Select(static error => error.Description).ToArray());

            return controller.ValidationProblem(new ValidationProblemDetails(validationErrors));
        }

        Error firstError = errors[0];
        int statusCode = firstError.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        Dictionary<string, object?> extensions = new()
        {
            ["errorCode"] = firstError.Code
        };

        if (errors.Count > 1)
        {
            extensions["errors"] = errors
                .Select(static error => new { error.Code, error.Description, Type = error.Type.ToString() })
                .ToArray();
        }

        return controller.Problem(
            statusCode: statusCode,
            title: firstError.Description,
            type: $"https://httpstatuses.com/{statusCode}",
            extensions: extensions);
    }
}
