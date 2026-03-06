using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SharedKernel;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authorization;

internal sealed class SchoolRoleAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserService currentUserService,
    ISchoolAccessService schoolAccessService)
    : AuthorizationHandler<SchoolRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SchoolRoleRequirement requirement)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            context.Fail();
            return;
        }

        SchoolId? schoolId = httpContext.GetSchoolIdFromRoute();
        if (schoolId is null)
        {
            context.Fail();
            return;
        }

        ICurrentUser currentUser = currentUserService.GetCurrentUser();
        if (!currentUser.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        bool hasRequiredRole = await schoolAccessService.HasAnyRoleAsync(
            currentUser.Id,
            schoolId.Value,
            requirement.AllowedRoles,
            httpContext.RequestAborted);

        if (hasRequiredRole)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
