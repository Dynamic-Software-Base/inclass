using Application.Abstractions.Messaging;
using Contract.InClass.Response.School.EducationalSystem;

namespace Application.EducationalSystem.Queries.GetDefaultEducationalSystem;

public sealed record GetDefaultEducationalSystemQuery : IQuery<ErrorOr<EducationalSystemResponse>>;
