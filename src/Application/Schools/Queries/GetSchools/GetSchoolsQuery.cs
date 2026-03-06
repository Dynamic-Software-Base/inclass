using Application.Abstractions.Messaging;
using Contract.InClass.Common;
using Contract.InClass.Pagination;
using Contract.InClass.Response;
using SharedKernel.ValueObjects.Schools;

namespace Application.Schools.Queries.GetSchools;

public record GetSchoolsQuery(string? NameSearch, string? City, GradeLevelOfferingDto? GradeLevel)
    : PagedRequest, IQuery<ErrorOr<PagedResult<SchoolSummaryDto>>>;
