using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Controller;

[ApiController]
[Route("api/auth-test")]
[Authorize] // Require valid JWT
public class AuthTestController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public AuthTestController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
#pragma warning disable S6968
    public IActionResult GetCurrentUser()
#pragma warning restore S6968
    {
        ICurrentUser user = _currentUserService.GetCurrentUser();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Roles,
            user.IsAuthenticated,
            user.IsSchoolOwner,
            user.IsTeacher,
            user.IsStudent,
            user.IsParent,
            user.IsPlatformAdmin
        });
    }

[HttpGet("school-owner-only")]
[Authorize(Roles = "school_owner")]
#pragma warning disable S6968
    public IActionResult SchoolOwnerOnly()
#pragma warning restore S6968
    {
        return Ok(new { message = "You are a school owner!" });
    }
}
