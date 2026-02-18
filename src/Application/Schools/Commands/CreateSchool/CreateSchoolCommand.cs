using Application.Schools.Contracts;
using MediatR;

namespace Application.Schools.Commands.CreateSchool;

public sealed record CreateSchoolCommand(string Name) : IRequest<ErrorOr<CreateSchoolResponse>>;
