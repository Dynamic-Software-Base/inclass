using Application.Abstractions.Messaging;
using Contract.InClass.Response.School;

namespace Application.Schools.Queries.GetOwnerSchools;

public record GetOwnerSchoolsQuery(): IQuery<ErrorOr<List<SchoolSummaryDto>>>;
