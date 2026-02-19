using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Authorization;

internal static class HttpContextExtensions
{
    internal static SchoolId? GetSchoolIdFromRoute(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        string? routeSchoolId = httpContext.GetRouteValue("schoolId")?.ToString();

        return Guid.TryParse(routeSchoolId, out Guid parsedSchoolId)
            ? SchoolId.From(parsedSchoolId)
            : null;
    }
}
