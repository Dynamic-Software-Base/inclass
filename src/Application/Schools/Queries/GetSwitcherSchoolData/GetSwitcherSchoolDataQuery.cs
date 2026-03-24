using Application.Abstractions.Messaging;
using Contract.InClass.Response.School;

namespace Application.Schools.Queries.GetSwitcherSchoolData;

public record GetSwitcherSchoolDataQuery():IQuery<ErrorOr<List<SchoolSwitcherDto>>>;
