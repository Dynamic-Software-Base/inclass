using Application.Schools.Contracts;
using MediatR;

namespace Application.Schools.Queries.GetMySchools;

public sealed record GetMySchoolsQuery : IRequest<ErrorOr<List<MySchoolDto>>>;
