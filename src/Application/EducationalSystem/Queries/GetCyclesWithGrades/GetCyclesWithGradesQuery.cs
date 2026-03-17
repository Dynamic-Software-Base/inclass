using Application.Abstractions.Messaging;
using Contract.InClass.Response.School.EducationalSystem;

namespace Application.EducationalSystem.Queries.GetCyclesWithGrades;

public sealed record GetCyclesWithGradesQuery(Guid EducationalSystemId)
    : IQuery<ErrorOr<List<CycleResponse>>>;
